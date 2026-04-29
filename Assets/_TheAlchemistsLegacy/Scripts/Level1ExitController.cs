using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1ExitController : MonoBehaviour
{
    [Header("Required Slot")]
    [SerializeField] private ItemSlotController requiredSealSlot;
    [SerializeField] private bool autoFindDoorSealSlot = true;

    [Header("Barrier")]
    [SerializeField] private GameObject[] barriersToDisable;
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
        if (!isUnlocked && requiredSealSlot != null && requiredSealSlot.IsFilled)
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

    private void UnlockExit()
    {
        isUnlocked = true;
        SetExitBlockersActive(false);
    }

    private void SetExitBlockersActive(bool isActive)
    {
        if (barriersToDisable == null)
        {
            return;
        }

        foreach (GameObject targetObject in barriersToDisable)
        {
            if (targetObject != null)
            {
                targetObject.SetActive(isActive);
            }
        }
    }

    private void AutoConfigureReferences()
    {
        if (autoFindDoorSealSlot && requiredSealSlot == null)
        {
            GameObject slotObject = GameObject.Find("DoorSealSlot_Level1");
            if (slotObject != null)
            {
                requiredSealSlot = slotObject.GetComponent<ItemSlotController>();
            }
        }

        if (autoFindTrapsFire && (barriersToDisable == null || barriersToDisable.Length == 0))
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
        Time.timeScale = 1.0f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPaused = false;
#endif
        SceneManager.LoadScene(sceneToLoad);
    }
}
