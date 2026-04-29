using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameMenu : MonoBehaviour
{
    private const string FirstLevelSceneName = "Level0";

    private void Start()
    {
        ResetRuntimeState();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        LoadSceneSafely(FirstLevelSceneName);
    }

    public void LoadLevelByName(string sceneName)
    {
        if (sceneName == "TeachLevel")
        {
            sceneName = FirstLevelSceneName;
        }

        LoadSceneSafely(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void LoadSceneSafely(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("No scene name was provided for the menu button.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogWarning($"Scene '{sceneName}' is not available. Add it to File > Build Settings > Scenes In Build, or create the scene asset first.");
            return;
        }

        ResetRuntimeState();
        SceneManager.LoadScene(sceneName);
    }

    private void ResetRuntimeState()
    {
        Time.timeScale = 1.0f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPaused = false;
#endif
    }
}
