using System;
using UnityEngine;

public class Level3ExitKeySlot : MonoBehaviour
{
    [Header("Accepted Item")]
    [SerializeField] private string acceptedKeyName = "Key_l3_exit";
    [SerializeField] private bool destroyKeyOnPlace = true;

    [Header("Slot Visual")]
    [SerializeField] private GameObject placedKeyVisual;
    [SerializeField] private Transform placedPoint;

    [Header("Exit Result")]
    [SerializeField] private GameObject[] objectsToEnable;
    [SerializeField] private GameObject[] objectsToDisable;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 3.0f;

    public bool IsFilled
    {
        get { return isFilled; }
    }

    private bool isFilled;
    private Camera playerCamera;
    private ItemPickup playerPickup;

    private void Start()
    {
        playerCamera = Camera.main;
        playerPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();

        if (placedKeyVisual != null)
        {
            placedKeyVisual.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isFilled)
        {
            TryPlaceKey();
        }
    }

    private void TryPlaceKey()
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

        if (!IsHoldingExitKey())
        {
            Debug.Log("StampSlot_l3_exit needs " + acceptedKeyName);
            return;
        }

        PlaceKey(playerPickup.currentItem);
        UnlockExit();
    }

    private bool IsHoldingExitKey()
    {
        return NameContains(playerPickup.currentItem.name, acceptedKeyName)
            || NameContains(playerPickup.currentItemName, acceptedKeyName);
    }

    private bool NameContains(string source, string expected)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(expected))
        {
            return false;
        }

        return source.IndexOf(expected, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private bool MatchesThisSlot(Transform hitTransform)
    {
        return hitTransform == transform || hitTransform.IsChildOf(transform);
    }

    private void PlaceKey(GameObject key)
    {
        isFilled = true;

        if (placedKeyVisual != null)
        {
            placedKeyVisual.SetActive(true);
        }

        if (destroyKeyOnPlace)
        {
            Destroy(key);
        }
        else
        {
            Transform targetPoint = placedPoint != null ? placedPoint : transform;
            key.transform.SetParent(targetPoint);
            key.transform.localPosition = Vector3.zero;
            key.transform.localRotation = Quaternion.identity;

            foreach (Collider keyCollider in key.GetComponentsInChildren<Collider>(true))
            {
                keyCollider.enabled = false;
            }

            Rigidbody keyBody = key.GetComponent<Rigidbody>();
            if (keyBody != null)
            {
                keyBody.isKinematic = true;
            }
        }

        playerPickup.currentItem = null;
        playerPickup.currentItemName = "";
        playerPickup.isHoldingItem = false;
    }

    private void UnlockExit()
    {
        SetObjectsActive(objectsToEnable, true);
        SetObjectsActive(objectsToDisable, false);
        Debug.Log("Level 3 exit unlocked.");
    }

    private void SetObjectsActive(GameObject[] objects, bool active)
    {
        if (objects == null)
        {
            return;
        }

        foreach (GameObject target in objects)
        {
            if (target != null)
            {
                target.SetActive(active);
            }
        }
    }
}
