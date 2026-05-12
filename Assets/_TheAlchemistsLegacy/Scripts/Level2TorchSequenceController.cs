using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2TorchSequenceController : MonoBehaviour
{
    [Header("Torch Sequence")]
    [SerializeField] private Level2TorchPoint[] torchesInOrder;
    [SerializeField] private bool autoFindTorches = true;
    [SerializeField] private string lightingToolName = "Torch_Level2";
    [SerializeField] private float interactionRange = 4f;
    [SerializeField] private float interactionAimRadius = 0.5f;

    [Header("Exit Unlock")]
    [SerializeField] private GameObject[] barriersToDisable;
    [SerializeField] private bool autoFindExitObjects = true;

    [Header("Birth Door Unlock")]
    [SerializeField] private bool unlockBirthDoorWhenTorchPicked = true;
    [SerializeField] private GameObject birthDoorObject;
    [SerializeField] private string birthDoorObjectName = "Door_birth";

    [Header("Scene Exit")]
    [SerializeField] private string sceneToLoad = "TrailComplete";
    [SerializeField] private bool loadOnPlayerTrigger = true;
    [SerializeField] private bool loadWhenPlayerIsNear = true;
    [SerializeField] private float exitDistance = 1.8f;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private Transform player;

    public bool ExitUnlocked => exitUnlocked;
    public bool BirthDoorUnlocked => birthDoorUnlocked;

    private Camera playerCamera;
    private ItemPickup itemPickup;
    private int nextTorchIndex;
    private bool exitUnlocked;
    private bool birthDoorUnlocked;
    private bool isLoading;

    private void Start()
    {
        AutoConfigureReferences();
        ResetTorchSequence();
        SetBarriersActive(true);
        LockBirthDoor();
    }

    private void Reset()
    {
        AutoConfigureReferences();
    }

    private void Update()
    {
        if (!birthDoorUnlocked && unlockBirthDoorWhenTorchPicked && PlayerIsHoldingLightingTool())
        {
            UnlockBirthDoor();
        }

        if (!exitUnlocked && Input.GetMouseButtonDown(1))
        {
            TryLightTorchFromView();
        }

        if (exitUnlocked && loadWhenPlayerIsNear)
        {
            TryLoadWhenPlayerIsNear();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!loadOnPlayerTrigger || !exitUnlocked || isLoading)
        {
            return;
        }

        if (IsPlayer(other))
        {
            LoadTargetScene();
        }
    }

    private void TryLightTorchFromView()
    {
        if (!PlayerIsHoldingLightingTool())
        {
            return;
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (playerCamera == null)
        {
            return;
        }

        if (!AimInteraction.Cast(playerCamera, interactionRange, interactionAimRadius, out RaycastHit hit))
        {
            return;
        }

        Level2TorchPoint torch = hit.collider.GetComponentInParent<Level2TorchPoint>();
        if (torch == null)
        {
            return;
        }

        TryLightTorch(torch);
    }

    private void TryLightTorch(Level2TorchPoint torch)
    {
        if (torch.IsLit)
        {
            return;
        }

        if (nextTorchIndex < torchesInOrder.Length && torch == torchesInOrder[nextTorchIndex])
        {
            torch.SetLit(true);
            nextTorchIndex++;

            if (nextTorchIndex >= torchesInOrder.Length)
            {
                UnlockExit();
            }

            return;
        }

        ResetTorchSequence();
        Debug.Log("Level 2 torch order broke. Begin again.");
    }

    private bool PlayerIsHoldingLightingTool()
    {
        if (itemPickup == null)
        {
            itemPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();
        }

        if (itemPickup == null || !itemPickup.isHoldingItem || itemPickup.currentItem == null)
        {
            return false;
        }

        return NameMatches(itemPickup.currentItemName, lightingToolName)
            || NameMatches(itemPickup.currentItem.name, lightingToolName);
    }

    private bool NameMatches(string source, string expected)
    {
        if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(expected))
        {
            return false;
        }

        return source.Equals(expected, StringComparison.OrdinalIgnoreCase)
            || source.IndexOf(expected, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private void UnlockExit()
    {
        exitUnlocked = true;
        SetBarriersActive(false);
        Debug.Log("Level 2 exit unlocked.");
    }

    private void ResetTorchSequence()
    {
        nextTorchIndex = 0;

        if (torchesInOrder == null)
        {
            return;
        }

        foreach (Level2TorchPoint torch in torchesInOrder)
        {
            if (torch != null)
            {
                torch.SetLit(false);
            }
        }
    }

    private void SetBarriersActive(bool isActive)
    {
        if (barriersToDisable == null)
        {
            return;
        }

        foreach (GameObject barrier in barriersToDisable)
        {
            if (barrier != null)
            {
                barrier.SetActive(isActive);
            }
        }
    }

    private void LockBirthDoor()
    {
        if (!unlockBirthDoorWhenTorchPicked)
        {
            return;
        }

        if (birthDoorObject == null)
        {
            birthDoorObject = FindSceneObjectByName(birthDoorObjectName);
        }

        if (birthDoorObject == null)
        {
            return;
        }

        foreach (Collider doorCollider in birthDoorObject.GetComponentsInChildren<Collider>(true))
        {
            doorCollider.enabled = true;
        }

        birthDoorUnlocked = false;
    }

    private void UnlockBirthDoor()
    {
        if (!unlockBirthDoorWhenTorchPicked)
        {
            return;
        }

        if (birthDoorObject == null)
        {
            birthDoorObject = FindSceneObjectByName(birthDoorObjectName);
        }

        if (birthDoorObject == null)
        {
            Debug.LogWarning("Level2TorchSequenceController could not find " + birthDoorObjectName + ".");
            return;
        }

        foreach (Collider doorCollider in birthDoorObject.GetComponentsInChildren<Collider>(true))
        {
            doorCollider.enabled = false;
        }

        birthDoorUnlocked = true;
        Debug.Log("Unlocked " + birthDoorObject.name + " after receiving " + lightingToolName + ".");
    }

    private GameObject FindSceneObjectByName(string objectName)
    {
        if (string.IsNullOrWhiteSpace(objectName))
        {
            return null;
        }

        Scene activeScene = SceneManager.GetActiveScene();
        foreach (GameObject rootObject in activeScene.GetRootGameObjects())
        {
            Transform match = FindChildByName(rootObject.transform, objectName);
            if (match != null)
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

    private void AutoConfigureReferences()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (itemPickup == null)
        {
            itemPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();
        }

        if (autoFindTorches && (torchesInOrder == null || torchesInOrder.Length == 0))
        {
            torchesInOrder = FindObjectsOfType<Level2TorchPoint>()
                .OrderBy(torch => torch.SequenceIndex)
                .ToArray();
        }

        if (autoFindExitObjects && (barriersToDisable == null || barriersToDisable.Length == 0))
        {
            barriersToDisable = new[]
            {
                GameObject.Find("Gate_Level2_Exit"),
                GameObject.Find("Traps_l2_exit")
            }.Where(target => target != null).ToArray();
        }

        if (birthDoorObject == null)
        {
            birthDoorObject = FindSceneObjectByName(birthDoorObjectName);
        }

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    private void TryLoadWhenPlayerIsNear()
    {
        if (isLoading)
        {
            return;
        }

        if (player == null)
        {
            AutoConfigureReferences();
        }

        if (player == null)
        {
            return;
        }

        Vector2 playerPosition = new Vector2(player.position.x, player.position.z);
        Vector2 exitPosition = new Vector2(transform.position.x, transform.position.z);

        if (Vector2.Distance(playerPosition, exitPosition) <= exitDistance)
        {
            LoadTargetScene();
        }
    }

    private bool IsPlayer(Collider other)
    {
        return other.CompareTag(playerTag) || other.transform.root.CompareTag(playerTag);
    }

    private void LoadTargetScene()
    {
        if (isLoading || string.IsNullOrWhiteSpace(sceneToLoad))
        {
            return;
        }

        isLoading = true;
        Time.timeScale = 1.0f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPaused = false;
#endif
        SceneManager.LoadScene(sceneToLoad);
    }
}
