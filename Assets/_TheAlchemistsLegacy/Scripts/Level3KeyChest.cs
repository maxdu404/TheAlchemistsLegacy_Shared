using System;
using UnityEngine;

public class Level3KeyChest : MonoBehaviour
{
    [Header("Required Key")]
    [SerializeField] private string keyItemName = "Key_l3_chest";
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

    public bool IsOpen
    {
        get { return isOpen; }
    }

    private bool isOpen;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Camera playerCamera;
    private ItemPickup playerPickup;

    private void Awake()
    {
        if (lid == null)
        {
            lid = FindLid();
        }
    }

    private void Start()
    {
        playerCamera = Camera.main;
        playerPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();
        SetContentsAvailable(false);

        if (lid != null)
        {
            closedRotation = lid.localRotation;
            openRotation = closedRotation * Quaternion.Euler(openAngle, 0.0f, 0.0f);
        }
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

        if (playerCamera == null || playerPickup == null)
        {
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (!Physics.Raycast(ray, out RaycastHit hit, interactionRange))
        {
            return;
        }

        if (!MatchesThisChest(hit.collider.transform))
        {
            return;
        }

        if (!IsHoldingKey())
        {
            Debug.Log("Chest_l3 needs " + keyItemName);
            return;
        }

        OpenChest();
    }

    private bool IsHoldingKey()
    {
        if (!playerPickup.isHoldingItem || playerPickup.currentItem == null)
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

        if (consumeKeyOnOpen)
        {
            Destroy(playerPickup.currentItem);
            playerPickup.currentItem = null;
            playerPickup.currentItemName = "";
            playerPickup.isHoldingItem = false;
        }

        SetContentsAvailable(true);
        Debug.Log("Chest_l3 opened.");
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
}
