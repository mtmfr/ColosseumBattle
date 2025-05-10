using UnityEngine;
using UnityEngine.UI;

public class UI_Pause : MonoBehaviour
{
    [Header("ResumeGameButton")]
    [SerializeField] private Button resumeGame;

    [Header("OptionMenuControl")]
    [SerializeField] private Button openOption;
    [SerializeField] private Button closeOption;

    [Header("MainMenuButton")]
    [SerializeField] private Button mainMenu;

    [Header("Menus")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionMenu;

    private void Start()
    {
        resumeGame.onClick.AddListener(Resume);
        mainMenu.onClick.AddListener(ToMainMenu);
        openOption.onClick.AddListener(OpenOptionMenu);
        closeOption.onClick.AddListener(CloseOptionMenu);
    }

    private void OnDestroy()
    {
        resumeGame.onClick.RemoveAllListeners();
        mainMenu.onClick.RemoveAllListeners();
        openOption.onClick.RemoveAllListeners();
        closeOption.onClick.RemoveAllListeners();
    }

    private void Resume() => GameManager.UpdateGameState(GameState.Wave);
    private void ToMainMenu()
    {
        ObjectPool.DiscardAllObject();
        GameManager.ResetScene();
    }

    private void OpenOptionMenu()
    {
        optionMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    private void CloseOptionMenu()
    {
        optionMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void SetActive(GameState gameState)
    {
        if (gameState == GameState.Pause)
            gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
}
