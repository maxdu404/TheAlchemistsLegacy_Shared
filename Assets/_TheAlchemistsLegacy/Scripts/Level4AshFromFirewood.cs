using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level4AshFromFirewood : MonoBehaviour
{
    [Header("Required Tool")]
    [SerializeField] private string requiredToolName = "forge_tool";
    [SerializeField] private bool consumeToolOnUse = true;

    [Header("Ash")]
    [SerializeField] private GameObject ashObject;
    [SerializeField] private string ashObjectName = "Ash_l4";
    [SerializeField] private bool hideAshUntilBurned = true;
    [SerializeField] private bool markAshAsPickup = true;

    [Header("Firewood")]
    [SerializeField] private bool hideFirewoodAfterBurn = false;
    [SerializeField] private GameObject magicFireObject;
    [SerializeField] private string magicFireObjectName = "Magic_fire_ash";
    [SerializeField] private bool hideMagicFireAfterBurn = true;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 3.0f;
    [SerializeField] private float interactionAimRadius = 0.5f;

    public bool IsBurned
    {
        get { return isBurned; }
    }

    public string RequiredToolName
    {
        get { return requiredToolName; }
    }

    private Camera playerCamera;
    private ItemPickup playerPickup;
    private bool isBurned;

    private void Start()
    {
        playerCamera = Camera.main;
        playerPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();

        if (ashObject == null)
        {
            ashObject = FindSceneObjectByName(ashObjectName);
        }

        if (magicFireObject == null)
        {
            magicFireObject = FindSceneObjectByName(magicFireObjectName);
        }

        if (ashObject != null && hideAshUntilBurned)
        {
            SetObjectAvailable(ashObject, false);
        }
    }

    private void Update()
    {
        if (isBurned || !Input.GetMouseButtonDown(1))
        {
            return;
        }

        TryBurnFirewood();
    }

    private void TryBurnFirewood()
    {
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

        if (!MatchesThisFirewood(hit.collider.transform))
        {
            return;
        }

        if (!IsHoldingRequiredTool())
        {
            Debug.Log(gameObject.name + " needs " + requiredToolName + ".");
            return;
        }

        BurnFirewood();
    }

    private bool MatchesThisFirewood(Transform hitTransform)
    {
        return hitTransform == transform || hitTransform.IsChildOf(transform);
    }

    private void BurnFirewood()
    {
        isBurned = true;

        if (ashObject == null)
        {
            ashObject = FindSceneObjectByName(ashObjectName);
        }

        if (ashObject != null)
        {
            if (markAshAsPickup)
            {
                TrySetTag(ashObject, "Pickup");
            }

            SetObjectAvailable(ashObject, true);
        }

        if (hideMagicFireAfterBurn)
        {
            if (magicFireObject == null)
            {
                magicFireObject = FindSceneObjectByName(magicFireObjectName);
            }

            if (magicFireObject != null)
            {
                SetObjectAvailable(magicFireObject, false);
            }
        }

        if (hideFirewoodAfterBurn)
        {
            foreach (Renderer firewoodRenderer in GetComponentsInChildren<Renderer>(true))
            {
                firewoodRenderer.enabled = false;
            }

            foreach (Collider firewoodCollider in GetComponentsInChildren<Collider>(true))
            {
                firewoodCollider.enabled = false;
            }
        }

        ConsumeHeldToolIfNeeded();
        Debug.Log("Level4 firewood burned into " + ashObjectName + ".");
    }

    private bool IsHoldingRequiredTool()
    {
        if (string.IsNullOrWhiteSpace(requiredToolName))
        {
            return true;
        }

        if (playerPickup == null)
        {
            playerPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();
        }

        if (playerPickup == null || !playerPickup.isHoldingItem || playerPickup.currentItem == null)
        {
            return false;
        }

        return NameContains(playerPickup.currentItem.name, requiredToolName)
            || NameContains(playerPickup.currentItemName, requiredToolName);
    }

    private void ConsumeHeldToolIfNeeded()
    {
        if (!consumeToolOnUse || playerPickup == null || playerPickup.currentItem == null)
        {
            return;
        }

        if (!IsHoldingRequiredTool())
        {
            return;
        }

        Destroy(playerPickup.currentItem);
        playerPickup.currentItem = null;
        playerPickup.currentItemName = "";
        playerPickup.isHoldingItem = false;
    }

    private bool NameContains(string source, string expected)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(expected))
        {
            return false;
        }

        return source.IndexOf(expected, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private void SetObjectAvailable(GameObject target, bool isAvailable)
    {
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

    private void TrySetTag(GameObject target, string tagName)
    {
        try
        {
            target.tag = tagName;
        }
        catch (UnityException)
        {
            Debug.LogWarning("Tag '" + tagName + "' does not exist. Add the tag or tag " + target.name + " manually.");
        }
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
            if (match != null && match.gameObject != gameObject)
            {
                return match.gameObject;
            }
        }

        return null;
    }

    private Transform FindChildByName(Transform root, string objectName)
    {
        if (root.name.Equals(objectName, StringComparison.OrdinalIgnoreCase))
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
}
