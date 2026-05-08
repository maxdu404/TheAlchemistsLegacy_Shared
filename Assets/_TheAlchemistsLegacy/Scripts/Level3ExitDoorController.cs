using UnityEngine;
using UnityEngine.SceneManagement;

public class Level3ExitDoorController : MonoBehaviour
{
    [Header("Required Slot")]
    [SerializeField] private Level3ExitKeySlot requiredExitSlot;
    [SerializeField] private string requiredExitSlotName = "StampSlot_l3_exit";

    [Header("Exit")]
    [SerializeField] private string sceneToLoad = "TrailComplete";
    [SerializeField] private bool loadOnPlayerTrigger = true;
    [SerializeField] private bool loadWhenPlayerIsNear = true;
    [SerializeField] private float exitDistance = 4.0f;
    [SerializeField] private string playerTag = "Player";

    private Transform player;
    private bool isLoading;

    private void Start()
    {
        AutoConfigureReferences();
    }

    private void Update()
    {
        if (IsExitUnlocked() && loadWhenPlayerIsNear)
        {
            TryLoadWhenPlayerIsNear();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!loadOnPlayerTrigger || isLoading || !IsExitUnlocked())
        {
            return;
        }

        if (IsPlayer(other))
        {
            LoadTargetScene();
        }
    }

    private bool IsExitUnlocked()
    {
        AutoConfigureReferences();
        return requiredExitSlot != null && requiredExitSlot.IsFilled;
    }

    private void AutoConfigureReferences()
    {
        if (requiredExitSlot == null)
        {
            GameObject slotObject = GameObject.Find(requiredExitSlotName);
            if (slotObject != null)
            {
                requiredExitSlot = slotObject.GetComponent<Level3ExitKeySlot>();
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
