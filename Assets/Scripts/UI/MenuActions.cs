using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuActions : MonoBehaviour
{
    [Tooltip("Exact name of the scene that starts the game.")]
    [SerializeField] private string gameSceneName = "WhiteBoxing";

    public void StartGame()
    {
        if (!Application.CanStreamedLevelBeLoaded(gameSceneName))
        {
            Debug.LogError("MENU ACTIONS - SCENE NOT FOUND OR NOT IN BUILD SETTINGS: " + gameSceneName);
            return;
        }

        Debug.Log("MENU ACTIONS - LOADING SCENE " + gameSceneName);
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("MENU ACTIONS - QUIT GAME");
        Application.Quit(); // does nothing in the editor, only in a built game
    }
}