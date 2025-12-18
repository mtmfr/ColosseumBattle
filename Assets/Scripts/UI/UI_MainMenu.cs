using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;
using TMPro;
using Unity.Collections;

public class UI_MainMenu : MonoBehaviour, IPointerClickHandler
{
    [Header("MenuWindows")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject credits;

    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [Space]
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitCreditsButton;

    private void OnEnable()
    {
        startButton.onClick.AddListener(StartGame);

        creditsButton.onClick.AddListener(ShowCredits);
        quitCreditsButton.onClick.AddListener(HideCredits);
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveAllListeners();

        creditsButton.onClick.RemoveAllListeners();
        quitCreditsButton.onClick.RemoveAllListeners();
    }

    private void StartGame()
    {
        GameManager.UpdateGameState(GameState.Start);
    }

    private void ShowCredits()
    {
        credits.SetActive(true);
        mainMenu.SetActive(false);
    }

    private void HideCredits()
    {
        credits.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject interactedObject = eventData.pointerPressRaycast.gameObject;

        if (interactedObject == null)
            return;

        if (!interactedObject.TryGetComponent(out TMP_Text text))
            return;

        int linkId = TMP_TextUtilities.FindIntersectingLink(text, eventData.position, null);
        if (linkId == -1)
            return;

        TMP_LinkInfo linkInfo = text.textInfo.linkInfo[linkId];

        string linkToOpen = linkInfo.GetLinkID();

        if (linkToOpen.Contains("https"))
            Application.OpenURL(linkToOpen);
    }

    public void SetActive(GameState gameState)
    {
        if (gameState == GameState.MainMenu)
            gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
}
