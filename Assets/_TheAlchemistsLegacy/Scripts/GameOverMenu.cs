using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    private const string FirstLevelSceneName = "Level0";

    private void Start()
    {
        ResetRuntimeState();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadLevelByName(string sceneName)
    {
        LoadSceneSafely(ResolveSceneName(sceneName));
    }

    public void RestartGame()
    {
        LoadSceneSafely(FirstLevelSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private string ResolveSceneName(string requestedSceneName)
    {
        GameObject selectedObject = EventSystem.current != null
            ? EventSystem.current.currentSelectedGameObject
            : null;

        if (selectedObject != null)
        {
            string buttonName = selectedObject.name.ToLowerInvariant();

            if (buttonName.Contains("level0") || buttonName.Contains("teach"))
            {
                return "Level0";
            }

            if (buttonName.Contains("level1"))
            {
                return "Level1";
            }

            if (buttonName.Contains("level2"))
            {
                return "Level2";
            }

            if (buttonName.Contains("level3"))
            {
                return "Level3";
            }

            if (buttonName.Contains("level4"))
            {
                return "Level4";
            }

            if (buttonName.Contains("level5"))
            {
                return "Level5";
            }
        }

        if (string.IsNullOrWhiteSpace(requestedSceneName))
        {
            return FirstLevelSceneName;
        }

        if (requestedSceneName == "TeachLevel")
        {
            return "Level0";
        }

        return requestedSceneName.Trim();
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
