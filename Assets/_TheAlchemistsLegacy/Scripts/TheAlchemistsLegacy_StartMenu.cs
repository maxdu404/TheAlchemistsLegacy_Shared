using UnityEngine;
using UnityEngine.SceneManagement;

public class TheAlchemistsLegacy_StartMenu : MonoBehaviour
{
    [SerializeField] private string firstLevelSceneName = "TheAlchemistsLegacy_Level0";

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1.0f;
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

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
