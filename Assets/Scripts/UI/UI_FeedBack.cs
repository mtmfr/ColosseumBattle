using System.Collections;
using TMPro;
using UnityEngine;

public class UI_FeedBack : MonoBehaviour
{
    private TextMeshProUGUI feedbackText;

    private void Start()
    {
        feedbackText = GetComponent<TextMeshProUGUI>();
    }

    public void ShowText(string textToShow, float duration)
    {
        if (feedbackText.color.a > 0.01f)
        {
            gameObject.SetActive(true);
            StartCoroutine(FadeOutText(textToShow, duration));
        }
    }

    public void HideTextPrematurly()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    private IEnumerator FadeOutText(string fadeText, float fadeDuration)
    {
        float passedTime = 1;
        feedbackText.text = fadeText;

        float passedTimeRatio = passedTime/fadeDuration;

        while (passedTimeRatio > 0.01f)
        {
            passedTime -= Time.deltaTime;

            float alphaValue = passedTimeRatio;

            feedbackText.color = new Color(1, 0, 0, alphaValue);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
