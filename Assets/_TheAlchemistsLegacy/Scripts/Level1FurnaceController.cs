using System;
using UnityEngine;

public class Level1FurnaceController : MonoBehaviour
{
    [Header("Required Items")]
    [SerializeField] private string woodItemName = "WoodLevel1";
    [SerializeField] private string metalItemName = "MetalLevel1";
    [SerializeField] private float interactionRange = 3.0f;

    [Header("Visuals")]
    [SerializeField] private GameObject woodPlacedVisual;
    [SerializeField] private GameObject metalPlacedVisual;

    [Header("Result")]
    [SerializeField] private GameObject finalSeal;

    private Camera playerCamera;
    private ItemPickup playerPickup;
    private bool hasWood;
    private bool hasMetal;
    private bool hasProducedSeal;

    private void Start()
    {
        playerCamera = Camera.main;
        playerPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();

        if (woodPlacedVisual != null)
        {
            woodPlacedVisual.SetActive(false);
        }

        if (metalPlacedVisual != null)
        {
            metalPlacedVisual.SetActive(false);
        }

        if (finalSeal != null)
        {
            finalSeal.SetActive(false);
        }
    }

    private void Update()
    {
        if (hasProducedSeal)
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

        if (!MatchesThisFurnace(hit.collider.transform))
        {
            return;
        }

        string currentItemName = playerPickup.currentItemName;
        string currentObjectName = playerPickup.currentItem.name;

        if (!hasWood && MatchesName(currentItemName, currentObjectName, woodItemName))
        {
            ConsumeHeldItem(isWood: true);
            return;
        }

        if (!hasMetal && MatchesName(currentItemName, currentObjectName, metalItemName))
        {
            ConsumeHeldItem(isWood: false);
            return;
        }
    }

    private bool MatchesThisFurnace(Transform hitTransform)
    {
        return hitTransform == transform || hitTransform.IsChildOf(transform);
    }

    private bool MatchesName(string currentItemName, string currentObjectName, string expectedName)
    {
        if (string.IsNullOrWhiteSpace(expectedName))
        {
            return false;
        }

        return ContainsName(currentItemName, expectedName) || ContainsName(currentObjectName, expectedName);
    }

    private bool ContainsName(string source, string expected)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return false;
        }

        return source.Equals(expected, StringComparison.OrdinalIgnoreCase)
            || source.IndexOf(expected, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private void ConsumeHeldItem(bool isWood)
    {
        GameObject heldItem = playerPickup.currentItem;

        if (isWood)
        {
            hasWood = true;

            if (woodPlacedVisual != null)
            {
                woodPlacedVisual.SetActive(true);
            }
        }
        else
        {
            hasMetal = true;

            if (metalPlacedVisual != null)
            {
                metalPlacedVisual.SetActive(true);
            }
        }

        Destroy(heldItem);

        playerPickup.currentItem = null;
        playerPickup.currentItemName = "";
        playerPickup.isHoldingItem = false;

        if (hasWood && hasMetal)
        {
            ProduceSeal();
        }
    }

    private void ProduceSeal()
    {
        hasProducedSeal = true;

        if (finalSeal != null)
        {
            finalSeal.SetActive(true);
        }

        Debug.Log("Level 1 furnace produced the final seal.");
    }
}
