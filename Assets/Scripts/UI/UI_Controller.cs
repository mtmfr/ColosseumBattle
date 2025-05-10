using UnityEngine;

public class UI_Controller : MonoBehaviour
{
    [SerializeField] private UI_MainMenu mainMenu;
    [SerializeField] private UI_Start start;
    [SerializeField] private UI_Game game;
    [SerializeField] private UI_Shop shop;
    [SerializeField] private UI_Pause pause;
    [SerializeField] private UI_GameOver gameOver;

    private void Awake()
    {
        GameManager.OnGameStateChange += mainMenu.SetActive;
        GameManager.OnGameStateChange += start.SetActive;
        GameManager.OnGameStateChange += game.SetActive;
        GameManager.OnGameStateChange += shop.SetActive;
        GameManager.OnGameStateChange += pause.SetActive;
        GameManager.OnGameStateChange += gameOver.SetActive;
    }

    private void OnEnable()
    {
        GameManager.UpdateGameState(GameState.MainMenu);
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= mainMenu.SetActive;
        GameManager.OnGameStateChange -= start.SetActive;
        GameManager.OnGameStateChange -= game.SetActive;
        GameManager.OnGameStateChange -= shop.SetActive;
        GameManager.OnGameStateChange -= pause.SetActive;
        GameManager.OnGameStateChange -= gameOver.SetActive;
    }
}
