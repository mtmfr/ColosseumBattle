using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    [Header("Loading")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider loadingProgressSlider;
    [SerializeField] private TextMeshProUGUI loadingProgressText;

    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            loadingProgressSlider.value = progress;
            loadingProgressText.text = progress * 100f + "%";

            yield return null;
            Old_GameManager.Instance.UpdateGameState(Old_GameState.Start);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}