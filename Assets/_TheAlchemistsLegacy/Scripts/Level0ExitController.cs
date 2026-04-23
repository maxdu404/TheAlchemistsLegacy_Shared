using UnityEngine;
using UnityEngine.SceneManagement;

public class Level0ExitController : MonoBehaviour
{
    [Header("Required Slots")]
    [SerializeField] private ItemSlotController[] requiredSlots;
    [SerializeField] private bool autoFindLevel0Slots = true;

    [Header("Barrier")]
    [SerializeField] private GameObject[] barriersToDisable;
    [SerializeField] private GameObject[] extraObjectsToDisable;
    [SerializeField] private bool autoFindTrapsFire = true;

    [Header("Exit")]
    [SerializeField] private string sceneToLoad = "TrailComplete";
    [SerializeField] private bool loadOnPlayerTrigger = true;
    [SerializeField] private bool loadWhenPlayerIsNear = true;
    [SerializeField] private float exitDistance = 1.6f;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private Transform player;

    private bool isUnlocked;
    private bool isLoading;

    private void Start()
    {
        AutoConfigureReferences();
        SetExitBlockersActive(true);
    }

    private void Reset()
    {
        AutoConfigureReferences();
    }

    private void Update()
    {
        if (!isUnlocked && AreAllSlotsFilled())
        {
            UnlockExit();
        }

        if (isUnlocked && loadWhenPlayerIsNear)
        {
            TryLoadWhenPlayerIsNear();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!loadOnPlayerTrigger || !isUnlocked || isLoading)
        {
            return;
        }

        if (IsPlayer(other))
        {
            LoadTargetScene();
        }
    }

    private bool AreAllSlotsFilled()
    {
        if (requiredSlots == null || requiredSlots.Length == 0)
        {
            return false;
        }

        foreach (ItemSlotController slot in requiredSlots)
        {
            if (slot == null || !slot.IsFilled)
            {
                return false;
            }
        }

        return true;
    }

    private void UnlockExit()
    {
        isUnlocked = true;
        SetExitBlockersActive(false);
    }

    private void SetExitBlockersActive(bool isActive)
    {
        SetObjectsActive(barriersToDisable, isActive);
        SetObjectsActive(extraObjectsToDisable, isActive);
    }

    private void SetObjectsActive(GameObject[] objects, bool isActive)
    {
        if (objects == null)
        {
            return;
        }

        foreach (GameObject targetObject in objects)
        {
            if (targetObject != null)
            {
                targetObject.SetActive(isActive);
            }
        }
    }

    private void AutoConfigureReferences()
    {
        if (autoFindLevel0Slots && HasMissingSlotReference())
        {
            ItemSlotController slot1 = FindSlotByName("StampSlot_1");
            ItemSlotController slot2 = FindSlotByName("StampSlot_2");

            if (slot1 != null && slot2 != null)
            {
                requiredSlots = new[] { slot1, slot2 };
            }
        }

        if (autoFindTrapsFire && HasMissingBarrierReference())
        {
            GameObject trapsFire = GameObject.Find("traps_fire");
            if (trapsFire != null)
            {
                barriersToDisable = new[] { trapsFire };
            }
        }

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    private ItemSlotController FindSlotByName(string slotName)
    {
        GameObject slotObject = GameObject.Find(slotName);
        if (slotObject == null)
        {
            return null;
        }

        return slotObject.GetComponent<ItemSlotController>();
    }

    private bool HasMissingSlotReference()
    {
        if (requiredSlots == null || requiredSlots.Length < 2)
        {
            return true;
        }

        foreach (ItemSlotController slot in requiredSlots)
        {
            if (slot == null)
            {
                return true;
            }
        }

        return false;
    }

    private bool HasMissingBarrierReference()
    {
        if (barriersToDisable == null || barriersToDisable.Length == 0)
        {
            return true;
        }

        foreach (GameObject barrier in barriersToDisable)
        {
            if (barrier == null)
            {
                return true;
            }
        }

        return false;
    }

    private void TryLoadWhenPlayerIsNear()
    {
        if (isLoading)
        {
            return;
        }

        if (player == null)
        {
            AutoConfigureReferences();
        }

        if (player == null)
        {
            return;
        }

        Vector2 playerPosition = new Vector2(player.position.x, player.position.z);
        Vector2 exitPosition = new Vector2(transform.position.x, transform.position.z);

        if (Vector2.Distance(playerPosition, exitPosition) <= exitDistance)
        {
            LoadTargetScene();
        }
    }

    private bool IsPlayer(Collider other)
    {
        return other.CompareTag(playerTag) || other.transform.root.CompareTag(playerTag);
    }

    private void LoadTargetScene()
    {
        if (isLoading || string.IsNullOrWhiteSpace(sceneToLoad))
        {
            return;
        }

        isLoading = true;
        SceneManager.LoadScene(sceneToLoad);
    }
}
