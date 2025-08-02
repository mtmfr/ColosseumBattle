using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;

    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
        startButton.onClick.AddListener(StartGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveAllListeners();
    }

    private void StartGame()
    {
        GameManager.UpdateGameState(GameState.Start);
    }

    private void QuitGame()
    {
#if UNITY_WEBGL

#elif UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SetActive(GameState gameState)
    {
        if (gameState == GameState.MainMenu)
            gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
}
