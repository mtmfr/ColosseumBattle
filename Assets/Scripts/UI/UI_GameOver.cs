using UnityEngine;
using UnityEngine.UI;

public class UI_GameOver : MonoBehaviour
{
    [SerializeField] private Button mainMenu;

    private void Start()
    {
        mainMenu.onClick.AddListener(ResetScene);
    }

    private void OnDestroy()
    {
        mainMenu.onClick.RemoveAllListeners();
    }

    private void ResetScene()
    {
       GameManager.ResetScene();
    }

    public void SetActive(GameState gameState)
    {
        if (gameState == GameState.GameOver)
            gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
}
