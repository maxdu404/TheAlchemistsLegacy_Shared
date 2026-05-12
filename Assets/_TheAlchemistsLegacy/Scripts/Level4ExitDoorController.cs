using UnityEngine;
using UnityEngine.SceneManagement;

public class Level4ExitDoorController : MonoBehaviour
{
    [Header("Completion Gate")]
    [SerializeField] private bool requireLevelCompletion = true;
    [SerializeField] private Level4CompletionController completionController;

    [Header("Exit")]
    [SerializeField] private string sceneToLoad = "TrailComplete";
    [SerializeField] private bool loadOnPlayerTrigger = true;
    [SerializeField] private bool loadWhenPlayerIsNear = true;
    [SerializeField] private float exitDistance = 3.0f;
    [SerializeField] private string playerTag = "Player";

    private Transform player;
    private bool isLoading;

    private void Start()
    {
        AutoConfigureReferences();
    }

    private void Update()
    {
        if (loadWhenPlayerIsNear && CanExit())
        {
            TryLoadWhenPlayerIsNear();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!loadOnPlayerTrigger || isLoading || !CanExit())
        {
            return;
        }

        if (IsPlayer(other))
        {
            LoadTargetScene();
        }
    }

    private bool CanExit()
    {
        if (isLoading)
        {
            return false;
        }

        if (!requireLevelCompletion)
        {
            return true;
        }

        AutoConfigureReferences();
        return completionController != null && completionController.IsCompleted;
    }

    private void AutoConfigureReferences()
    {
        if (completionController == null)
        {
            completionController = FindObjectOfType<Level4CompletionController>();
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
