using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Pick Up Settings")]
    public KeyCode pickupKey = KeyCode.E;
    public float pickupRange = 2.0f;
    public Transform handPosition;
    public LayerMask pickupLayer;

    [Header("Drop Settings")]
    [SerializeField] private float dropDistance = 1.2f;
    [SerializeField] private float dropHeightOffset = 0.2f;
    [SerializeField] private float dropForwardImpulse = 0.5f;

    [Header("Recall Settings")]
    [Tooltip("Press this key with empty hands to recall the most recently picked-up item. Items consumed by puzzles (destroyed or hidden) cannot be recalled.")]
    [SerializeField] private KeyCode recallKey = KeyCode.R;

    private readonly List<GameObject> recallableItems = new List<GameObject>();

    [Header("Current Item")]
    public GameObject currentItem;
    public bool isHoldingItem = false;

    [Header("Current Item Name")]
    public string currentItemName = "";

    public static ItemPickup instance;

    private Camera playerCamera;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerCamera = Camera.main;

        if (handPosition == null)
        {
            Debug.LogError("ItemPickup needs a handPosition assigned in the Inspector.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(recallKey) && !isHoldingItem)
        {
            RecallLastItem();
            return;
        }

        if (!Input.GetKeyDown(pickupKey))
        {
            return;
        }

        if (isHoldingItem)
        {
            DropItem();
        }
        else
        {
            TryPickupItem();
        }
    }

    private void TryPickupItem()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (playerCamera == null)
        {
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (!Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickupLayer))
        {
            return;
        }

        GameObject pickupObject = FindPickupRoot(hit.collider);
        if (pickupObject == null)
        {
            Debug.Log("Object cannot be picked up: " + hit.collider.name);
            return;
        }

        PickupItem(pickupObject);
    }

    private GameObject FindPickupRoot(Collider hitCollider)
    {
        Transform target = hitCollider.transform;

        while (target != null)
        {
            if (target.CompareTag("Pickup"))
            {
                Rigidbody rootBody = target.GetComponentInParent<Rigidbody>();
                if (rootBody != null && rootBody.CompareTag("Pickup"))
                {
                    return rootBody.gameObject;
                }

                return target.gameObject;
            }

            target = target.parent;
        }

        return null;
    }

    private void PickupItem(GameObject item)
    {
        if (item == null || handPosition == null)
        {
            return;
        }

        currentItem = item;
        currentItemName = item.name;
        isHoldingItem = true;

        if (!recallableItems.Contains(item))
        {
            recallableItems.Add(item);
        }

        Rigidbody itemRigidbody = item.GetComponent<Rigidbody>();
        if (itemRigidbody != null)
        {
            itemRigidbody.velocity = Vector3.zero;
            itemRigidbody.angularVelocity = Vector3.zero;
            itemRigidbody.isKinematic = true;
        }

        SetItemCollidersEnabled(item, false);

        item.transform.SetParent(handPosition, false);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        Debug.Log("Picked up item: " + item.name);
    }

    private void DropItem()
    {
        if (currentItem == null)
        {
            ClearHeldItem();
            return;
        }

        // Defensive registration: catches items that entered the hand via
        // giver scripts that bypass PickupItem() (e.g. Level3TorchGiver).
        if (!recallableItems.Contains(currentItem))
        {
            recallableItems.Add(currentItem);
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (playerCamera == null)
        {
            currentItem.transform.SetParent(null);
            ClearHeldItem();
            return;
        }

        currentItem.transform.SetParent(null);

        Vector3 dropPosition = ComputeSafeDropPosition();
        currentItem.transform.position = dropPosition;
        currentItem.transform.rotation = Quaternion.LookRotation(playerCamera.transform.forward, Vector3.up);

        SetItemCollidersEnabled(currentItem, true);
        EnsureConvexMeshColliders(currentItem);
        Physics.SyncTransforms();

        Rigidbody itemRigidbody = currentItem.GetComponent<Rigidbody>();
        if (itemRigidbody != null)
        {
            itemRigidbody.velocity = Vector3.zero;
            itemRigidbody.angularVelocity = Vector3.zero;
            itemRigidbody.useGravity = true;
            itemRigidbody.isKinematic = false;
            // Discrete is safe for all collider types (including non-convex mesh).
            // ContinuousDynamic requires convex shapes and caused crashes with some items.
            itemRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            itemRigidbody.AddForce(playerCamera.transform.forward * dropForwardImpulse, ForceMode.Impulse);
        }

        Debug.Log("Dropped item: " + currentItem.name);
        ClearHeldItem();
    }

    private Vector3 ComputeSafeDropPosition()
    {
        Vector3 origin = playerCamera.transform.position;
        Vector3 forward = playerCamera.transform.forward;

        float safeDistance = dropDistance;
        if (Physics.Raycast(origin, forward, out RaycastHit wallHit, dropDistance + 0.2f))
        {
            // keep 0.15 m clearance from the wall so the item doesn't embed in it
            safeDistance = Mathf.Max(0.3f, wallHit.distance - 0.15f);
        }

        return origin + forward * safeDistance + Vector3.up * dropHeightOffset;
    }

    private void SetItemCollidersEnabled(GameObject item, bool enabled)
    {
        Collider[] itemColliders = item.GetComponentsInChildren<Collider>(true);
        foreach (Collider itemCollider in itemColliders)
        {
            itemCollider.enabled = enabled;
        }
    }

    // Non-convex MeshColliders are forbidden on non-kinematic Rigidbodies (Unity 5+).
    // Convert them to convex when the item is about to be dropped.
    private void EnsureConvexMeshColliders(GameObject item)
    {
        MeshCollider[] meshColliders = item.GetComponentsInChildren<MeshCollider>(true);
        foreach (MeshCollider mc in meshColliders)
        {
            if (!mc.convex)
            {
                mc.convex = true;
            }
        }
    }

    private void ClearHeldItem()
    {
        currentItem = null;
        currentItemName = "";
        isHoldingItem = false;
    }

    // Walk back through items the player has picked up before and recall the most
    // recent one that is still recoverable. Skip items destroyed (consumed by a
    // puzzle) or hidden via SetActive(false) (placed on a pedestal/slot).
    private void RecallLastItem()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (playerCamera == null)
        {
            return;
        }

        for (int i = recallableItems.Count - 1; i >= 0; i--)
        {
            GameObject item = recallableItems[i];

            if (item == null)
            {
                recallableItems.RemoveAt(i);
                continue;
            }

            if (!item.activeInHierarchy)
            {
                continue;
            }

            RecallItem(item);
            return;
        }

        Debug.Log("Recall: nothing to summon.");
    }

    private void RecallItem(GameObject item)
    {
        item.transform.SetParent(null);
        item.transform.position = ComputeSafeDropPosition();
        item.transform.rotation = Quaternion.LookRotation(playerCamera.transform.forward, Vector3.up);

        SetItemCollidersEnabled(item, true);
        EnsureConvexMeshColliders(item);
        Physics.SyncTransforms();

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        }

        Debug.Log("Recalled item: " + item.name);
    }
}
