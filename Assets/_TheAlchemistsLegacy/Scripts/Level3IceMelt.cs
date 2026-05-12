using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level3IceMelt : MonoBehaviour
{
    [Header("Required Item")]
    [InspectorName("Required Held Item Name")]
    [SerializeField] private string requiredTorchName = "torch_for_ice_l3";

    [Header("Result")]
    [InspectorName("Revealed Object")]
    [SerializeField] private GameObject lanternBaseObject;
    [InspectorName("Revealed Object Name")]
    [SerializeField] private string lanternBaseObjectName = "lantern_base_l3";
    [InspectorName("Hide Revealed Object Until Melted")]
    [SerializeField] private bool hideLanternBaseUntilMelted = true;
    [SerializeField] private GameObject[] extraObjectsToHide;
    [SerializeField] private GameObject[] extraObjectsToReveal;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 3.0f;
    [SerializeField] private float interactionAimRadius = 0.5f;

    public bool IsMelted
    {
        get { return isMelted; }
    }

    private bool isMelted;
    private Camera playerCamera;
    private ItemPickup playerPickup;

    private void Start()
    {
        ConfigureFromName();
        playerCamera = Camera.main;
        playerPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();

        if (lanternBaseObject == null)
        {
            lanternBaseObject = FindSceneObjectByName(lanternBaseObjectName);
        }

        if (hideLanternBaseUntilMelted && lanternBaseObject != null)
        {
            SetObjectAvailable(lanternBaseObject, false);
        }

        SetObjectsAvailable(extraObjectsToReveal, false);
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
        if (lowerName.Contains("ice_l2s") || lowerName.Contains("ice_l2"))
        {
            requiredTorchName = "torch_for_ice_l2";
            lanternBaseObjectName = "YellowStone_l4";
            hideLanternBaseUntilMelted = true;
        }
    }

    private void Update()
    {
        if (isMelted || !Input.GetMouseButtonDown(1))
        {
            return;
        }

        TryMeltIce();
    }

    private void TryMeltIce()
    {
        RefreshReferences();

        if (playerCamera == null || playerPickup == null)
        {
            return;
        }

        if (!IsHoldingRequiredItem())
        {
            return;
        }

        if (!AimInteraction.Cast(playerCamera, interactionRange, interactionAimRadius, out RaycastHit hit))
        {
            return;
        }

        if (!MatchesThisIce(hit.collider.transform))
        {
            return;
        }

        MeltIce();
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

        if (lanternBaseObject == null)
        {
            lanternBaseObject = FindSceneObjectByName(lanternBaseObjectName);
        }
    }

    private bool IsHoldingRequiredItem()
    {
        if (playerPickup == null || !playerPickup.isHoldingItem || playerPickup.currentItem == null)
        {
            return false;
        }

        return NameContains(playerPickup.currentItem.name, requiredTorchName)
            || NameContains(playerPickup.currentItemName, requiredTorchName)
            || NameContains(playerPickup.currentItem.name, "torch_for_ice_l3")
            || NameContains(playerPickup.currentItemName, "torch_for_ice_l3")
            || NameContains(playerPickup.currentItem.name, "torch_for_ice_l2")
            || NameContains(playerPickup.currentItemName, "torch_for_ice_l2");
    }

    private bool MatchesThisIce(Transform hitTransform)
    {
        return hitTransform == transform || hitTransform.IsChildOf(transform);
    }

    private void MeltIce()
    {
        isMelted = true;

        SetObjectAvailable(lanternBaseObject, true);
        SetObjectsAvailable(extraObjectsToReveal, true);
        SetObjectsAvailable(extraObjectsToHide, false);

        SetObjectAvailable(gameObject, false);
        Debug.Log(gameObject.name + " melted. The revealed object is available.");
    }

    private bool NameContains(string source, string expected)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(expected))
        {
            return false;
        }

        return source.IndexOf(expected, StringComparison.OrdinalIgnoreCase) >= 0;
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

    private void SetObjectsAvailable(GameObject[] objects, bool isAvailable)
    {
        if (objects == null)
        {
            return;
        }

        foreach (GameObject target in objects)
        {
            SetObjectAvailable(target, isAvailable);
        }
    }

    private void SetObjectAvailable(GameObject target, bool isAvailable)
    {
        if (target == null)
        {
            return;
        }

        target.SetActive(isAvailable);

        foreach (Collider targetCollider in target.GetComponentsInChildren<Collider>(true))
        {
            targetCollider.enabled = isAvailable;
        }

        foreach (Rigidbody targetBody in target.GetComponentsInChildren<Rigidbody>(true))
        {
            targetBody.isKinematic = !isAvailable;
        }
    }
}
