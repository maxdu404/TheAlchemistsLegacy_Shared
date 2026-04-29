using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TheAlchemistsLegacy_TrialComplete : MonoBehaviour
{
    [SerializeField] private string title = "Trial Complete";
    [SerializeField] private string subtitle = "Choose your next trial.";
    [SerializeField] private string startMenuSceneName = "TrailComplete";
    [SerializeField] private string firstLevelSceneName = "Level0";
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI subtitleText;

    private void Start()
    {
        ResetRuntimeState();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (titleText != null)
        {
            titleText.text = title;
        }

        if (subtitleText != null)
        {
            subtitleText.text = subtitle;
        }
    }

    public void StartGame()
    {
        LoadScene(firstLevelSceneName);
    }

    public void LoadLevelByName(string sceneName)
    {
        if (!string.IsNullOrWhiteSpace(sceneName))
        {
            LoadScene(sceneName);
        }
    }

    public void ReturnToStartMenu()
    {
        LoadScene(startMenuSceneName);
    }

    private void LoadScene(string sceneName)
    {
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

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
