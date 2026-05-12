using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level4CastleEntryGateController : MonoBehaviour
{
    [Header("Required Outside Placements")]
    [SerializeField] private string[] requiredSlotNames = { "Sign_Ash_l4", "Sign_Salt" };
    [SerializeField] private string[] requiredPlacedVisualNames = { "Ash_l4_Placed", "Salt_Placed" };
    [SerializeField] private bool usePlacedVisualFallbackIfSlotMissing = false;

    [Header("Castle Entry Doors")]
    [SerializeField] private string[] gateObjectNames = { "Door_Middle_1_th", "Door_Middle_1th2" };
    [SerializeField] private bool closeGateUntilRequirementsMet = true;

    private ItemSlotController[] requiredSlots;
    private GameObject[] requiredPlacedVisuals;
    private GameObject[] gateObjects;
    private bool gateOpened;

    private void Start()
    {
        ResolveSceneReferences();
        UpdateGateState();
    }

    private void Update()
    {
        if (gateOpened)
        {
            return;
        }

        UpdateGateState();
    }

    private void ResolveSceneReferences()
    {
        requiredSlots = new ItemSlotController[requiredSlotNames.Length];
        for (int i = 0; i < requiredSlotNames.Length; i++)
        {
            GameObject slotObject = FindSceneObjectByName(requiredSlotNames[i]);
            if (slotObject != null)
            {
                requiredSlots[i] = FindSlotController(slotObject, requiredSlotNames[i]);
            }

            if (requiredSlots[i] == null)
            {
                requiredSlots[i] = FindSlotControllerByName(requiredSlotNames[i]);
            }
        }

        requiredPlacedVisuals = new GameObject[requiredPlacedVisualNames.Length];
        for (int i = 0; i < requiredPlacedVisualNames.Length; i++)
        {
            requiredPlacedVisuals[i] = FindSceneObjectByName(requiredPlacedVisualNames[i]);
        }

        gateObjects = new GameObject[gateObjectNames.Length];
        for (int i = 0; i < gateObjectNames.Length; i++)
        {
            gateObjects[i] = FindSceneObjectByName(gateObjectNames[i]);
        }
    }

    private void UpdateGateState()
    {
        bool requirementsMet = AreOutsidePlacementsComplete();

        if (requirementsMet)
        {
            OpenGate();
            return;
        }

        if (closeGateUntilRequirementsMet)
        {
            SetGateActive(true);
        }
    }

    private bool AreOutsidePlacementsComplete()
    {
        for (int i = 0; i < requiredSlotNames.Length; i++)
        {
            if (HasSlot(i))
            {
                if (IsSlotFilled(i))
                {
                    continue;
                }

                return false;
            }

            if (usePlacedVisualFallbackIfSlotMissing && IsPlacedVisualActive(i))
            {
                continue;
            }

            return false;
        }

        return true;
    }

    private bool HasSlot(int index)
    {
        return requiredSlots != null
            && index >= 0
            && index < requiredSlots.Length
            && requiredSlots[index] != null;
    }

    private bool IsSlotFilled(int index)
    {
        if (requiredSlots == null || index < 0 || index >= requiredSlots.Length)
        {
            return false;
        }

        ItemSlotController slot = requiredSlots[index];
        return slot != null && slot.IsFilled;
    }

    private bool IsPlacedVisualActive(int index)
    {
        if (requiredPlacedVisuals == null || index < 0 || index >= requiredPlacedVisuals.Length)
        {
            return false;
        }

        GameObject placedVisual = requiredPlacedVisuals[index];
        return placedVisual != null && placedVisual.activeInHierarchy;
    }

    private ItemSlotController FindSlotController(GameObject slotObject, string slotName)
    {
        ItemSlotController slot = slotObject.GetComponent<ItemSlotController>();
        if (slot != null)
        {
            return slot;
        }

        slot = slotObject.GetComponentInParent<ItemSlotController>();
        if (slot != null)
        {
            return slot;
        }

        slot = slotObject.GetComponentInChildren<ItemSlotController>(true);
        if (slot != null)
        {
            return slot;
        }

        return FindSlotControllerByName(slotName);
    }

    private ItemSlotController FindSlotControllerByName(string slotName)
    {
        string expectedName = NormalizeName(slotName);
        ItemSlotController[] sceneSlots = FindObjectsOfType<ItemSlotController>(true);
        foreach (ItemSlotController slot in sceneSlots)
        {
            if (slot == null)
            {
                continue;
            }

            if (NormalizeName(slot.gameObject.name).Equals(expectedName, StringComparison.OrdinalIgnoreCase))
            {
                return slot;
            }
        }

        return null;
    }

    private void OpenGate()
    {
        gateOpened = true;
        SetGateActive(false);
        Debug.Log("Level4 castle entry opened after Ash_l4 and Salt_l4 were placed.");
    }

    private void SetGateActive(bool active)
    {
        if (gateObjects == null)
        {
            return;
        }

        for (int i = 0; i < gateObjects.Length; i++)
        {
            GameObject gateObject = gateObjects[i];
            if (gateObject == null)
            {
                continue;
            }

            if (gateObject.activeSelf != active)
            {
                gateObject.SetActive(active);
            }
        }
    }

    private GameObject FindSceneObjectByName(string objectName)
    {
        if (string.IsNullOrWhiteSpace(objectName))
        {
            return null;
        }

        string expectedName = NormalizeName(objectName);
        Scene activeScene = SceneManager.GetActiveScene();
        foreach (GameObject rootObject in activeScene.GetRootGameObjects())
        {
            Transform match = FindChildByName(rootObject.transform, expectedName);
            if (match != null)
            {
                return match.gameObject;
            }
        }

        return null;
    }

    private Transform FindChildByName(Transform root, string expectedName)
    {
        if (NormalizeName(root.name).Equals(expectedName, StringComparison.OrdinalIgnoreCase))
        {
            return root;
        }

        foreach (Transform child in root)
        {
            Transform match = FindChildByName(child, expectedName);
            if (match != null)
            {
                return match;
            }
        }

        return null;
    }

    private string NormalizeName(string objectName)
    {
        return objectName.Trim().Replace("(Clone)", "").Trim();
    }
}
