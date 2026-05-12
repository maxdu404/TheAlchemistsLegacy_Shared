using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyChestController : MonoBehaviour
{
    [Header("Required Key")]
    [SerializeField] private bool requiresKey = false;
    [SerializeField] private string keyItemName = "Key_l3_chest";
    [SerializeField] private bool autoConfigureLevel4SaltChest = true;
    [SerializeField] private bool consumeKeyOnOpen = true;

    [Header("Chest")]
    [SerializeField] private Transform lid;
    [SerializeField] private float openAngle = -90.0f;
    [SerializeField] private float openSpeed = 2.0f;

    [Header("Contents")]
    [SerializeField] private GameObject[] contentsToReveal;
    [SerializeField] private bool hideContentsUntilOpen = true;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 3.0f;
    [SerializeField] private float interactionAimRadius = 0.5f;

    public bool IsOpen
    {
        get { return isOpen; }
    }

    public bool RequiresKey
    {
        get { return requiresKey; }
    }

    public string KeyItemName
    {
        get { return keyItemName; }
    }

    private bool isOpen;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Camera playerCamera;
    private ItemPickup playerPickup;

    private void Reset()
    {
        ConfigureFromName();
    }

    private void OnValidate()
    {
        ConfigureFromName();
    }

    private void Awake()
    {
        ConfigureFromName();
        ConfigureLevel4ContentsFromName();

        if (lid == null)
        {
            lid = FindLid();
        }
    }

    private void Start()
    {
        ConfigureFromName();
        ConfigureLevel4ContentsFromName();
        playerCamera = Camera.main;
        playerPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();
        SetContentsAvailable(false);

        if (lid != null)
        {
            closedRotation = lid.localRotation;
            openRotation = closedRotation * Quaternion.Euler(openAngle, 0.0f, 0.0f);
        }
    }

    private void ConfigureFromName()
    {
        if (gameObject.name.IndexOf("Chest_l4_salt", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            requiresKey = true;
            keyItemName = "Key_l4_salt";
            return;
        }

        if (gameObject.name.IndexOf("Chest_l3", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            requiresKey = true;
            keyItemName = "Key_l3_chest";
        }
    }

    private void ConfigureLevel4ContentsFromName()
    {
        if (!autoConfigureLevel4SaltChest)
        {
            return;
        }

        if (gameObject.name.IndexOf("Chest_l4_salt", StringComparison.OrdinalIgnoreCase) < 0)
        {
            return;
        }

        if (HasContentsAssigned())
        {
            return;
        }

        GameObject salt = FindSceneObjectByName("Salt_l4");
        if (salt != null)
        {
            contentsToReveal = new[] { salt };
        }
    }

    private bool HasContentsAssigned()
    {
        if (contentsToReveal == null || contentsToReveal.Length == 0)
        {
            return false;
        }

        foreach (GameObject content in contentsToReveal)
        {
            if (content != null)
            {
                return true;
            }
        }

        return false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isOpen)
        {
            TryOpenChest();
        }

        if (lid != null)
        {
            Quaternion targetRotation = isOpen ? openRotation : closedRotation;
            lid.localRotation = Quaternion.Slerp(lid.localRotation, targetRotation, Time.deltaTime * openSpeed);
        }
    }

    private void TryOpenChest()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (playerPickup == null)
        {
            playerPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();
        }

        if (playerCamera == null)
        {
            return;
        }

        if (!AimInteraction.Cast(playerCamera, interactionRange, interactionAimRadius, out RaycastHit hit))
        {
            return;
        }

        if (!MatchesThisChest(hit.collider.transform))
        {
            return;
        }

        if (requiresKey && !IsHoldingKey())
        {
            Debug.Log(gameObject.name + " needs " + keyItemName);
            return;
        }

        OpenChest();
    }

    private bool IsHoldingKey()
    {
        if (!requiresKey)
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

        return NameContains(playerPickup.currentItem.name, keyItemName)
            || NameContains(playerPickup.currentItemName, keyItemName);
    }

    private bool NameContains(string source, string expected)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(expected))
        {
            return false;
        }

        return source.IndexOf(expected, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private bool MatchesThisChest(Transform hitTransform)
    {
        return hitTransform == transform || hitTransform.IsChildOf(transform);
    }

    private void OpenChest()
    {
        if (isOpen)
        {
            return;
        }

        isOpen = true;

        if (requiresKey && consumeKeyOnOpen && playerPickup != null && playerPickup.currentItem != null)
        {
            Destroy(playerPickup.currentItem);
            playerPickup.currentItem = null;
            playerPickup.currentItemName = "";
            playerPickup.isHoldingItem = false;
        }

        SetLidCollidersEnabled(false);
        SetContentsAvailable(true);
        Debug.Log(gameObject.name + " opened.");
    }

    private void SetLidCollidersEnabled(bool enabled)
    {
        if (lid == null)
        {
            return;
        }

        foreach (Collider col in lid.GetComponentsInChildren<Collider>(true))
        {
            col.enabled = enabled;
        }
    }

    private void SetContentsAvailable(bool isAvailable)
    {
        if (contentsToReveal == null)
        {
            return;
        }

        foreach (GameObject content in contentsToReveal)
        {
            if (content == null)
            {
                continue;
            }

            if (hideContentsUntilOpen)
            {
                content.SetActive(isAvailable);
            }

            foreach (Collider contentCollider in content.GetComponentsInChildren<Collider>(true))
            {
                contentCollider.enabled = isAvailable;
            }

            foreach (Rigidbody contentBody in content.GetComponentsInChildren<Rigidbody>(true))
            {
                contentBody.isKinematic = !isAvailable;
            }
        }
    }

    private Transform FindLid()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            string lowerName = child.name.ToLowerInvariant();
            if (lowerName.Contains("lid") || lowerName.Contains("top"))
            {
                return child;
            }
        }

        return null;
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
