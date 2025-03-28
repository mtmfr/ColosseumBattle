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
        Old_GameManager.Instance.OnGameStateChanged += UpdateCurrentUI;
    }

    private void OnDisable()
    {
        Old_GameManager.Instance.OnGameStateChanged -= UpdateCurrentUI;
    }

    private void UpdateCurrentUI(Old_GameState gameState)
    {
        switch (gameState)
        {
            case Old_GameState.Start:
                StartUI.SetActive(true);
                GameUI.SetActive(false);
                ShopUI.SetActive(false);
                GameOverUI.SetActive(false);
                break;
            case Old_GameState.Fight:
                StartUI.SetActive(false);
                GameUI.SetActive(true);
                ShopUI.SetActive(false);
                GameOverUI.SetActive(false);
                break;
            case Old_GameState.Shop:
                StartUI.SetActive(false);
                GameUI.SetActive(false);
                ShopUI.SetActive(true);
                GameOverUI.SetActive(false);
                break;
            case Old_GameState.Lose:
                StartUI.SetActive(false);
                GameUI.SetActive(false);
                ShopUI.SetActive(false);
                GameOverUI.SetActive(true);
                break;
        }
    }
}
