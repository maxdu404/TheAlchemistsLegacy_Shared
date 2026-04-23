using System;
using UnityEngine;

public class ItemSlotController : MonoBehaviour
{
    [Header("Accepted Item")]
    [SerializeField] private string acceptedItemName = "Stamp1";
    [SerializeField] private bool autoConfigureFromSlotName = true;

    [Header("Slot Visual")]
    [SerializeField] private GameObject placedVisual;
    [SerializeField] private Transform placedPoint;
    [SerializeField] private bool destroyHeldItemOnPlace = true;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 3.0f;

    public bool IsFilled
    {
        get { return isFilled; }
    }

    public string AcceptedItemName
    {
        get { return acceptedItemName; }
    }

    private bool isFilled;
    private Camera playerCamera;
    private ItemPickup playerPickup;

    private void Reset()
    {
        ConfigureAcceptedItemFromSlotName();
    }

    private void OnValidate()
    {
        ConfigureAcceptedItemFromSlotName();
    }

    private void Start()
    {
        ConfigureAcceptedItemFromSlotName();
        playerCamera = Camera.main;
        playerPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();

        if (placedVisual != null)
        {
            placedVisual.SetActive(false);
        }
    }

    private void ConfigureAcceptedItemFromSlotName()
    {
        if (!autoConfigureFromSlotName)
        {
            return;
        }

        string lowerName = gameObject.name.ToLowerInvariant();

        if (lowerName.Contains("slot_1") || lowerName.Contains("slot1"))
        {
            acceptedItemName = "Stamp1";
        }
        else if (lowerName.Contains("slot_2") || lowerName.Contains("slot2"))
        {
            acceptedItemName = "Stamp2";
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isFilled)
        {
            TryPlaceHeldItem();
        }
    }

    private void TryPlaceHeldItem()
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

        if (!playerPickup.isHoldingItem || playerPickup.currentItem == null)
        {
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (!Physics.Raycast(ray, out RaycastHit hit, interactionRange))
        {
            return;
        }

        if (!MatchesThisSlot(hit.collider.transform))
        {
            return;
        }

        if (!MatchesAcceptedItem(playerPickup.currentItem))
        {
            return;
        }

        PlaceItem(playerPickup.currentItem);
        ClearHeldItem();
        isFilled = true;
    }

    private bool MatchesThisSlot(Transform hitTransform)
    {
        return hitTransform == transform || hitTransform.IsChildOf(transform);
    }

    private bool MatchesAcceptedItem(GameObject item)
    {
        if (item == null || string.IsNullOrWhiteSpace(acceptedItemName))
        {
            return false;
        }

        string expectedName = acceptedItemName.Trim();
        string itemName = item.name.Trim();
        string currentItemName = playerPickup != null ? playerPickup.currentItemName.Trim() : "";

        return itemName.Equals(expectedName, StringComparison.OrdinalIgnoreCase)
            || itemName.IndexOf(expectedName, StringComparison.OrdinalIgnoreCase) >= 0
            || currentItemName.Equals(expectedName, StringComparison.OrdinalIgnoreCase)
            || currentItemName.IndexOf(expectedName, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private void PlaceItem(GameObject item)
    {
        if (placedVisual != null)
        {
            placedVisual.SetActive(true);
        }

        if (destroyHeldItemOnPlace)
        {
            Destroy(item);
            return;
        }

        Transform targetPoint = placedPoint != null ? placedPoint : transform;
        item.transform.SetParent(targetPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        Collider itemCollider = item.GetComponent<Collider>();
        if (itemCollider != null)
        {
            itemCollider.enabled = false;
        }

        Rigidbody itemBody = item.GetComponent<Rigidbody>();
        if (itemBody != null)
        {
            itemBody.isKinematic = true;
        }
    }

    private void ClearHeldItem()
    {
        playerPickup.currentItem = null;
        playerPickup.currentItemName = "";
        playerPickup.isHoldingItem = false;
    }
}
