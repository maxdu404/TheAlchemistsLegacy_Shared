using UnityEngine;
using UnityEngine.SceneManagement;

public class Level4CompletionController : MonoBehaviour
{
    [Header("Placed Visuals to Watch (all must be active)")]
    [SerializeField] private string[] placedVisualNames = { "Ash_l4_Placed", "Salt_Placed", "YellowStone_l4_Placed", "BlueDrop_Placed" };

    [Header("Objects to Enable on Completion")]
    [SerializeField] private string[] objectsToEnableNames = { "plate_sowrd", "sword" };

    [Header("Objects to Disable on Completion")]
    [SerializeField] private string[] objectsToDisableNames = { "Traps_l4_exit" };

    [Header("Exit Door")]
    [SerializeField] private string exitDoorName = "Door_l4_exit";
    [SerializeField] private float exitProximityRange = 3.0f;
    [SerializeField] private string sceneToLoad = "TrailComplete";
    [SerializeField] private bool loadExitWhenNear = false;

    private bool isCompleted;
    private bool isLoading;
    private Transform playerTransform;
    private Transform exitDoor;
    private GameObject[] placedVisuals;

    public bool IsCompleted => isCompleted;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        placedVisuals = new GameObject[placedVisualNames.Length];
        for (int i = 0; i < placedVisualNames.Length; i++)
        {
            placedVisuals[i] = GameObject.Find(placedVisualNames[i]);
        }

        foreach (string objName in objectsToEnableNames)
        {
            GameObject obj = GameObject.Find(objName);
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (!isCompleted)
        {
            CheckCompletion();
            return;
        }

        if (loadExitWhenNear)
        {
            TryExitWhenNear();
        }
    }

    private void CheckCompletion()
    {
        for (int i = 0; i < placedVisuals.Length; i++)
        {
            GameObject visual = placedVisuals[i];

            if (visual == null)
            {
                visual = GameObject.Find(placedVisualNames[i]);
                placedVisuals[i] = visual;
            }

            if (visual == null || !visual.activeInHierarchy)
            {
                return;
            }
        }

        TriggerCompletion();
    }

    private void TriggerCompletion()
    {
        isCompleted = true;

        foreach (string objName in objectsToEnableNames)
        {
            GameObject obj = GameObject.Find(objName);
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        foreach (string objName in objectsToDisableNames)
        {
            GameObject obj = GameObject.Find(objName);
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        GameObject door = GameObject.Find(exitDoorName);
        if (door != null)
        {
            exitDoor = door.transform;
        }

        TheAlchemistsLegacyLevelHud hud = FindObjectOfType<TheAlchemistsLegacyLevelHud>();
        if (hud != null)
        {
            hud.ShowMessage(
                "Legacy Restored",
                "The four signs answer as one.\nThe castle accepts your inheritance."
            );
        }

        Debug.Log("Level 4 complete — all materials placed.");
    }

    private void TryExitWhenNear()
    {
        if (isLoading)
        {
            return;
        }

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                return;
            }

            playerTransform = player.transform;
        }

        if (exitDoor == null)
        {
            return;
        }

        float distance = Vector3.Distance(playerTransform.position, exitDoor.position);
        if (distance <= exitProximityRange)
        {
            LoadExitScene();
        }
    }

    private void LoadExitScene()
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
