using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlUI : MonoBehaviour
{
    [SerializeField] private GameObject StartUI;
    [SerializeField] private GameObject GameUI;
    [SerializeField] private GameObject ShopUI;
    [SerializeField] private GameObject GameOverUI;

    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += UpdateCurrentUI;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameStateChanged -= UpdateCurrentUI;
    }

    private void UpdateCurrentUI(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Start:
                StartUI.SetActive(true);
                GameUI.SetActive(false);
                ShopUI.SetActive(false);
                GameOverUI.SetActive(false);
                break;
            case GameState.Fight:
                StartUI.SetActive(false);
                GameUI.SetActive(true);
                ShopUI.SetActive(false);
                GameOverUI.SetActive(false);
                break;
            case GameState.Shop:
                StartUI.SetActive(false);
                GameUI.SetActive(false);
                ShopUI.SetActive(true);
                GameOverUI.SetActive(false);
                break;
            case GameState.Lose:
                StartUI.SetActive(false);
                GameUI.SetActive(false);
                ShopUI.SetActive(false);
                GameOverUI.SetActive(true);
                break;
        }
    }
}
