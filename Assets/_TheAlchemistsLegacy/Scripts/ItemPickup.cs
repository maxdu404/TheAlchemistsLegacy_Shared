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

        currentItem.transform.SetParent(null);

        Vector3 dropPosition = playerCamera.transform.position
            + playerCamera.transform.forward * dropDistance
            + Vector3.up * dropHeightOffset;

        currentItem.transform.position = dropPosition;
        currentItem.transform.rotation = Quaternion.LookRotation(playerCamera.transform.forward, Vector3.up);

        SetItemCollidersEnabled(currentItem, true);
        Physics.SyncTransforms();

        Rigidbody itemRigidbody = currentItem.GetComponent<Rigidbody>();
        if (itemRigidbody != null)
        {
            itemRigidbody.velocity = Vector3.zero;
            itemRigidbody.angularVelocity = Vector3.zero;
            itemRigidbody.useGravity = true;
            itemRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            itemRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            itemRigidbody.isKinematic = false;
            itemRigidbody.AddForce(playerCamera.transform.forward * dropForwardImpulse, ForceMode.Impulse);
        }

        Debug.Log("Dropped item: " + currentItem.name);
        ClearHeldItem();
    }

    private void SetItemCollidersEnabled(GameObject item, bool enabled)
    {
        Collider[] itemColliders = item.GetComponentsInChildren<Collider>(true);
        foreach (Collider itemCollider in itemColliders)
        {
            itemCollider.enabled = enabled;
        }
    }

    private void ClearHeldItem()
    {
        currentItem = null;
        currentItemName = "";
        isHoldingItem = false;
    }
}
