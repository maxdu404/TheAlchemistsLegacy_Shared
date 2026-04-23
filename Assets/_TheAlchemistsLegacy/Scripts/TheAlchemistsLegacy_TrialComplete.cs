using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TheAlchemistsLegacy_TrialComplete : MonoBehaviour
{
    [SerializeField] private string title = "Trial Complete";
    [SerializeField] private string subtitle = "Choose your next trial.";
    [SerializeField] private string startMenuSceneName = "NewMenu";
    [SerializeField] private string firstLevelSceneName = "TheAlchemistsLegacy_Level0";
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI subtitleText;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1.0f;

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
        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void LoadLevelByName(string sceneName)
    {
        if (!string.IsNullOrWhiteSpace(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    public void ReturnToStartMenu()
    {
        SceneManager.LoadScene(startMenuSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
