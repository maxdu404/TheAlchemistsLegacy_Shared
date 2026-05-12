using UnityEngine;
using UnityEngine.SceneManagement;

public class Level3TorchGiver : MonoBehaviour
{
    [Header("Torch")]
    [SerializeField] private GameObject torchForIcePrefabOrObject;
    [SerializeField] private string heldTorchName = "torch_for_ice_l3";
    [SerializeField] private bool instantiateTorch = true;
    [SerializeField] private bool giveOnlyOnce = false;
    [SerializeField] private bool lightTorchWhenGiven = true;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 3.0f;
    [SerializeField] private float interactionAimRadius = 0.5f;

    [Header("Birth Door Unlock")]
    [SerializeField] private bool unlockBirthDoorWhenTorchGiven = true;
    [SerializeField] private GameObject birthDoorObject;
    [SerializeField] private string birthDoorObjectName = "Door_birth";
    [SerializeField] private string alternateBirthDoorObjectName = "Door_1_Venge";

    private Camera playerCamera;
    private ItemPickup playerPickup;
    private bool hasGivenTorch;

    private void Start()
    {
        ConfigureFromName();
        playerCamera = Camera.main;
        playerPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();

        if (torchForIcePrefabOrObject == null)
        {
            torchForIcePrefabOrObject = FindSceneObjectByName(heldTorchName);
        }
        else if (ShouldReplaceAssignedTorchObject())
        {
            GameObject intendedTorch = FindSceneObjectByName(heldTorchName);
            if (intendedTorch != null)
            {
                torchForIcePrefabOrObject = intendedTorch;
            }
        }

        if (birthDoorObject == null)
        {
            birthDoorObject = FindBirthDoorObject();
        }
    }

    private void Reset()
    {
        ConfigureFromName();
    }

    private void OnValidate()
    {
        ConfigureFromName();
    }

    private void ConfigureFromName()
    {
        string lowerName = gameObject.name.ToLowerInvariant();
        if (lowerName.Contains("candle_l2"))
        {
            heldTorchName = "torch_for_ice_l2";
            instantiateTorch = true;
            unlockBirthDoorWhenTorchGiven = false;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            TryGiveTorch();
        }
    }

    private void TryGiveTorch()
    {
        if (giveOnlyOnce && hasGivenTorch)
        {
            return;
        }

        RefreshReferences();

        if (playerCamera == null || playerPickup == null || playerPickup.handPosition == null)
        {
            Debug.LogWarning("Level3TorchGiver is missing the player camera, ItemPickup, or hand position.");
            return;
        }

        if (!AimInteraction.Cast(playerCamera, interactionRange, interactionAimRadius, out RaycastHit hit))
        {
            return;
        }

        if (!MatchesThisObject(hit.collider.transform))
        {
            return;
        }

        if (playerPickup.isHoldingItem)
        {
            Debug.Log("Drop your current item before taking the torch.");
            return;
        }

        GameObject torch = CreateTorchObject();
        if (torch == null)
        {
            Debug.LogWarning("Level3TorchGiver could not find or create torch_for_ice_l3.");
            return;
        }

        GiveTorchToPlayer(torch);
        UnlockBirthDoor();
        hasGivenTorch = true;
    }

    private void RefreshReferences()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (playerPickup == null)
        {
            playerPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();
        }

        if (torchForIcePrefabOrObject == null)
        {
            torchForIcePrefabOrObject = FindSceneObjectByName(heldTorchName);
        }
        else if (ShouldReplaceAssignedTorchObject())
        {
            GameObject intendedTorch = FindSceneObjectByName(heldTorchName);
            if (intendedTorch != null)
            {
                torchForIcePrefabOrObject = intendedTorch;
            }
        }

        if (birthDoorObject == null)
        {
            birthDoorObject = FindBirthDoorObject();
        }
    }

    private bool MatchesThisObject(Transform hitTransform)
    {
        return hitTransform == transform || hitTransform.IsChildOf(transform);
    }

    private bool ShouldReplaceAssignedTorchObject()
    {
        if (torchForIcePrefabOrObject == null)
        {
            return false;
        }

        if (torchForIcePrefabOrObject == gameObject)
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(heldTorchName))
        {
            return false;
        }

        return torchForIcePrefabOrObject.name.IndexOf(heldTorchName, System.StringComparison.OrdinalIgnoreCase) < 0
            && gameObject.name.IndexOf("candle_l2", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private GameObject CreateTorchObject()
    {
        if (ShouldReplaceAssignedTorchObject())
        {
            torchForIcePrefabOrObject = FindSceneObjectByName(heldTorchName);
        }

        if (torchForIcePrefabOrObject == null)
        {
            return null;
        }

        GameObject torch = instantiateTorch
            ? Instantiate(torchForIcePrefabOrObject)
            : torchForIcePrefabOrObject;

        torch.name = heldTorchName;
        torch.SetActive(true);
        return torch;
    }

    private void GiveTorchToPlayer(GameObject torch)
    {
        Rigidbody body = torch.GetComponent<Rigidbody>();
        if (body != null)
        {
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.isKinematic = true;
            body.constraints = RigidbodyConstraints.FreezeRotation;
            body.angularDrag = 8.0f;
        }

        Collider[] colliders = torch.GetComponentsInChildren<Collider>(true);
        foreach (Collider torchCollider in colliders)
        {
            torchCollider.enabled = false;
        }

        torch.transform.SetParent(playerPickup.handPosition, false);
        torch.transform.localPosition = Vector3.zero;
        torch.transform.localRotation = Quaternion.identity;


        if (lightTorchWhenGiven)
        {
            torch.SendMessage("Ignite", SendMessageOptions.DontRequireReceiver);
            EnableFlameChildren(torch);
        }

        playerPickup.currentItem = torch;
        playerPickup.currentItemName = heldTorchName;
        playerPickup.isHoldingItem = true;

        Debug.Log("Received " + heldTorchName);
    }

    private void UnlockBirthDoor()
    {
        if (!unlockBirthDoorWhenTorchGiven)
        {
            return;
        }

        if (birthDoorObject == null)
        {
            birthDoorObject = FindBirthDoorObject();
        }

        if (birthDoorObject == null)
        {
            Debug.LogWarning("Level3TorchGiver could not find " + birthDoorObjectName + " or " + alternateBirthDoorObjectName);
            return;
        }

        foreach (Collider doorCollider in birthDoorObject.GetComponentsInChildren<Collider>(true))
        {
            doorCollider.enabled = false;
        }

        Debug.Log("Unlocked " + birthDoorObject.name + " after receiving " + heldTorchName);
    }

    private GameObject FindBirthDoorObject()
    {
        GameObject match = FindSceneObjectByName(birthDoorObjectName);
        if (match != null)
        {
            return match;
        }

        return FindSceneObjectByName(alternateBirthDoorObjectName);
    }

    private GameObject FindSceneObjectByName(string objectName)
    {
        if (string.IsNullOrEmpty(objectName))
        {
            return null;
        }

        Scene activeScene = SceneManager.GetActiveScene();
        foreach (GameObject rootObject in activeScene.GetRootGameObjects())
        {
            Transform match = FindChildByName(rootObject.transform, objectName);
            if (match != null && match.gameObject != gameObject)
            {
                return match.gameObject;
            }
        }

        return null;
    }

    private Transform FindChildByName(Transform root, string objectName)
    {
        if (root.name == objectName)
        {
            return root;
        }

        foreach (Transform child in root)
        {
            Transform match = FindChildByName(child, objectName);
            if (match != null)
            {
                return match;
            }
        }

        return null;
    }

    private void EnableFlameChildren(GameObject torch)
    {
        foreach (Transform child in torch.GetComponentsInChildren<Transform>(true))
        {
            string lowerName = child.name.ToLowerInvariant();
            if (lowerName.Contains("flame") || lowerName.Contains("fire"))
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}
