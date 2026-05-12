using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level4MaterialSlot : MonoBehaviour
{
    [Header("Accepted Item")]
    [SerializeField] private string acceptedItemName = "";
    [SerializeField] private bool consumeItemOnPlace = true;

    [Header("Visual")]
    [SerializeField] private GameObject placedVisual;
    [SerializeField] private string placedVisualName = "";

    [Header("Doors to Disable When All Slots Filled")]
    [SerializeField] private string[] doorObjectNames = { "Door_Middle_1_th", "Door_Middle_1th2" };

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 3.0f;
    [SerializeField] private float interactionAimRadius = 0.5f;

    public bool IsFilled { get; private set; }

    private Camera playerCamera;
    private ItemPickup playerPickup;

    private void Start()
    {
        playerCamera = Camera.main;
        playerPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();

        if (placedVisual == null && !string.IsNullOrWhiteSpace(placedVisualName))
        {
            placedVisual = FindSceneObjectByName(placedVisualName);
        }

        if (placedVisual != null)
        {
            placedVisual.SetActive(false);
        }
    }

    private void Update()
    {
        if (IsFilled || !Input.GetMouseButtonDown(1))
        {
            return;
        }

        TryPlace();
    }

    private void TryPlace()
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

        if (!AimInteraction.Cast(playerCamera, interactionRange, interactionAimRadius, out RaycastHit hit))
        {
            return;
        }

        if (!MatchesThis(hit.collider.transform))
        {
            return;
        }

        if (!IsHoldingAccepted())
        {
            Debug.Log(gameObject.name + " needs " + acceptedItemName);
            return;
        }

        PlaceItem();
        CheckAllSlotsFilled();
    }

    private bool MatchesThis(Transform hitTransform)
    {
        return hitTransform == transform || hitTransform.IsChildOf(transform);
    }

    private bool IsHoldingAccepted()
    {
        if (string.IsNullOrWhiteSpace(acceptedItemName))
        {
            return true;
        }

        return NameContains(playerPickup.currentItem.name, acceptedItemName)
            || NameContains(playerPickup.currentItemName, acceptedItemName);
    }

    private void PlaceItem()
    {
        IsFilled = true;

        if (placedVisual == null && !string.IsNullOrWhiteSpace(placedVisualName))
        {
            placedVisual = FindSceneObjectByName(placedVisualName);
        }

        if (placedVisual != null)
        {
            placedVisual.SetActive(true);
        }

        if (consumeItemOnPlace && playerPickup.currentItem != null)
        {
            Destroy(playerPickup.currentItem);
            playerPickup.currentItem = null;
            playerPickup.currentItemName = "";
            playerPickup.isHoldingItem = false;
        }

        Debug.Log(gameObject.name + ": placed " + acceptedItemName);
    }

    private void CheckAllSlotsFilled()
    {
        Level4MaterialSlot[] allSlots = FindObjectsOfType<Level4MaterialSlot>();
        foreach (Level4MaterialSlot slot in allSlots)
        {
            if (!slot.IsFilled)
            {
                return;
            }
        }

        foreach (string doorName in doorObjectNames)
        {
            if (string.IsNullOrWhiteSpace(doorName))
            {
                continue;
            }

            GameObject door = FindSceneObjectByName(doorName);
            if (door != null)
            {
                door.SetActive(false);
            }
        }

        Debug.Log("All material slots filled — doors disabled.");
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
        if (string.IsNullOrWhiteSpace(objectName))
        {
            return null;
        }

        Scene activeScene = SceneManager.GetActiveScene();
        foreach (GameObject root in activeScene.GetRootGameObjects())
        {
            Transform match = FindChildByName(root.transform, objectName);
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
