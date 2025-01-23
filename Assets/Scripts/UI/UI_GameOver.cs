using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_GameOver : MonoBehaviour
{

    [Header("Game Over")]
    [SerializeField] private GameObject GameOverBG;
    [SerializeField] private float fadeDuration;

    [SerializeField] private GameObject GOButtons;

    private void OnEnable()
    {
        WaveEvent.GameOver += ShowGameOverScreen;
    }

    private void OnDisable()
    {
        WaveEvent.GameOver -= ShowGameOverScreen;
    }

    private void ShowGameOverScreen()
    {
        StartCoroutine(GameOverFade());
    }

    public void Restart()
    {
        GameManager.Instance.UpdateGameState(GameState.Start);
        SceneManager.LoadScene(1);
    }

    public void GoBackToMenu()
    {
        GameManager.Instance.UpdateGameState(GameState.MainMenu);
        SceneManager.LoadScene(0);
    }

    private IEnumerator GameOverFade()
    {
        for (float alpha = 0; alpha < 1; alpha += Time.fixedDeltaTime)
        {
            GameOverBG.GetComponent<Image>().color = new Color(0, 0, 0, alpha);
        }
        yield return new WaitForSeconds(fadeDuration);
        GOButtons.SetActive(true);
    }
}
