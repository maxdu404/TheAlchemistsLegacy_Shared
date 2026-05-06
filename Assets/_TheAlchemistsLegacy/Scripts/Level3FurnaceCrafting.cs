using System;
using UnityEngine;

public class Level3FurnaceCrafting : MonoBehaviour
{
    [Header("Required Ingredients")]
    [SerializeField] private string oilItemName = "FireBall_l3";
    [SerializeField] private string baseItemName = "lantern_base_l3";
    [SerializeField] private string wickItemName = "l3_wick";

    [Header("Ingredient Visuals")]
    [SerializeField] private GameObject oilPlacedVisual;
    [SerializeField] private GameObject basePlacedVisual;
    [SerializeField] private GameObject wickPlacedVisual;

    [Header("Crafted Lamp")]
    [SerializeField] private GameObject lampObject;
    [SerializeField] private Transform lampSpawnPoint;
    [SerializeField] private bool hideLampUntilCrafted = true;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 3.0f;

    public bool IsCrafted
    {
        get { return isCrafted; }
    }

    private bool hasOil;
    private bool hasBase;
    private bool hasWick;
    private bool isCrafted;
    private Camera playerCamera;
    private ItemPickup playerPickup;

    private void Start()
    {
        playerCamera = Camera.main;
        playerPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();
        SetVisual(oilPlacedVisual, false);
        SetVisual(basePlacedVisual, false);
        SetVisual(wickPlacedVisual, false);

        if (lampObject != null && hideLampUntilCrafted)
        {
            SetObjectAvailable(lampObject, false);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isCrafted)
        {
            TryAddIngredient();
        }
    }

    private void TryAddIngredient()
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

        GameObject heldItem = playerPickup.currentItem;
        if (!hasOil && MatchesHeldItem(oilItemName))
        {
            hasOil = true;
            AcceptIngredient(heldItem, oilPlacedVisual);
        }
        else if (!hasBase && MatchesHeldItem(baseItemName))
        {
            hasBase = true;
            AcceptIngredient(heldItem, basePlacedVisual);
        }
        else if (!hasWick && MatchesHeldItem(wickItemName))
        {
            hasWick = true;
            AcceptIngredient(heldItem, wickPlacedVisual);
        }
        else
        {
            Debug.Log("furnace_l3 does not need this item now.");
            return;
        }

        if (hasOil && hasBase && hasWick)
        {
            CraftLamp();
        }
    }

    private bool MatchesHeldItem(string expectedName)
    {
        if (string.IsNullOrEmpty(expectedName) || playerPickup == null)
        {
            return false;
        }

        string itemName = playerPickup.currentItem != null ? playerPickup.currentItem.name : "";
        string storedName = playerPickup.currentItemName;

        return NameContains(itemName, expectedName) || NameContains(storedName, expectedName);
    }

    private bool NameContains(string source, string expected)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(expected))
        {
            return false;
        }

        return source.IndexOf(expected, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private bool MatchesThisFurnace(Transform hitTransform)
    {
        return hitTransform == transform || hitTransform.IsChildOf(transform);
    }

    private void AcceptIngredient(GameObject heldItem, GameObject placedVisual)
    {
        SetVisual(placedVisual, true);
        Destroy(heldItem);
        playerPickup.currentItem = null;
        playerPickup.currentItemName = "";
        playerPickup.isHoldingItem = false;
    }

    private void CraftLamp()
    {
        isCrafted = true;

        if (lampObject != null)
        {
            if (lampSpawnPoint != null)
            {
                lampObject.transform.position = lampSpawnPoint.position;
                lampObject.transform.rotation = lampSpawnPoint.rotation;
            }

            SetObjectAvailable(lampObject, true);
        }

        Debug.Log("lamp_l3 crafted.");
    }

    private void SetVisual(GameObject visual, bool active)
    {
        if (visual != null)
        {
            visual.SetActive(active);
        }
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
}
