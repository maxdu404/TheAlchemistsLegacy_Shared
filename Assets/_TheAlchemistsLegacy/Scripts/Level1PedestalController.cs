using System;
using System.Collections.Generic;
using UnityEngine;

public class Level1PedestalController : MonoBehaviour
{
    [Header("Required Items")]
    [SerializeField] private string[] requiredItems = { "Stamp1", "Stamp2", "Stamp3" };
    [SerializeField] private GameObject[] placedVisuals;
    [SerializeField] private float interactionRange = 3.0f;

    [Header("Unlock Targets")]
    [SerializeField] private GameObject cageLevel1;
    [SerializeField] private GameObject fireLevel1;
    [SerializeField] private Level1ScrollReader furnaceScroll;

    private readonly HashSet<string> placedItems = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    private Camera playerCamera;
    private ItemPickup playerPickup;
    private bool isUnlocked;

    public int PlacedCount
    {
        get { return placedItems.Count; }
    }

    public bool IsUnlocked
    {
        get { return isUnlocked; }
    }

    private void Start()
    {
        playerCamera = Camera.main;
        playerPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();

        if (placedVisuals != null)
        {
            foreach (GameObject visual in placedVisuals)
            {
                if (visual != null)
                {
                    visual.SetActive(false);
                }
            }
        }

        if (fireLevel1 != null)
        {
            fireLevel1.SetActive(false);
        }

        if (furnaceScroll != null)
        {
            furnaceScroll.SetReadable(false);
        }
    }

    private void Update()
    {
        if (isUnlocked)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
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

        if (!MatchesThisReceiver(hit.collider.transform))
        {
            return;
        }

        string matchedItem = FindMatchingRequiredItem(playerPickup.currentItemName, playerPickup.currentItem.name);
        if (string.IsNullOrEmpty(matchedItem))
        {
            return;
        }

        PlaceHeldItem(matchedItem, playerPickup.currentItem);
    }

    private bool MatchesThisReceiver(Transform hitTransform)
    {
        return hitTransform == transform || hitTransform.IsChildOf(transform);
    }

    private string FindMatchingRequiredItem(string displayName, string objectName)
    {
        foreach (string required in requiredItems)
        {
            if (placedItems.Contains(required))
            {
                continue;
            }

            if (NameMatches(displayName, required) || NameMatches(objectName, required))
            {
                return required;
            }
        }

        return null;
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

    private void PlaceHeldItem(string matchedItem, GameObject heldItem)
    {
        placedItems.Add(matchedItem);

        int visualIndex = Array.FindIndex(
            requiredItems,
            item => string.Equals(item, matchedItem, StringComparison.OrdinalIgnoreCase));

        if (placedVisuals != null &&
            visualIndex >= 0 &&
            visualIndex < placedVisuals.Length &&
            placedVisuals[visualIndex] != null)
        {
            placedVisuals[visualIndex].SetActive(true);
        }

        Destroy(heldItem);

        playerPickup.currentItem = null;
        playerPickup.currentItemName = "";
        playerPickup.isHoldingItem = false;

        if (placedItems.Count >= requiredItems.Length)
        {
            UnlockNextStage();
        }
    }

    private void UnlockNextStage()
    {
        isUnlocked = true;

        if (cageLevel1 != null)
        {
            cageLevel1.SetActive(false);
        }

        if (fireLevel1 != null)
        {
            fireLevel1.SetActive(true);
        }

        if (furnaceScroll != null)
        {
            furnaceScroll.SetReadable(true);
        }

        Debug.Log("Level 1 forge unlocked.");
    }
}
