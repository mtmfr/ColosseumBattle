using UnityEngine;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;

    [SerializeField] private Button startButton;

    private void OnEnable()
    {
        startButton.onClick.AddListener(StartGame);
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveAllListeners();
    }

    private void StartGame()
    {
        GameManager.UpdateGameState(GameState.Start);
    }

    public void SetActive(GameState gameState)
    {
        if (gameState == GameState.MainMenu)
            gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
}
