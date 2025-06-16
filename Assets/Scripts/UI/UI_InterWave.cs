using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_InterWave : MonoBehaviour
{
    [SerializeField] private InterWave interWave;
    [SerializeField] private HeroShop heroShop;
    [SerializeField] private PartyController partyController;

    #region UnityFunctions
    private void Awake()
    {
        interWave.Initialize();
        heroShop.Initialize();
        partyController.Initialize();

        interWave.cancelButton.onClick.AddListener(Cancel);

        heroShop.HeroBought += partyController.UpdateFrames;
    }

    private void OnEnable()
    {
        heroShop.Enable();
        partyController.UpdateFrames();
    }

    private void OnDestroy()
    {
        interWave.Destroy();
        heroShop.Destroy();
        partyController.Destroy();

        interWave.cancelButton.onClick.RemoveListener(Cancel);

        heroShop.HeroBought -= partyController.UpdateFrames;
    }
    #endregion

    private void Cancel()
    {
        switch (interWave.currentState)
        {
            case UIState.Shop:
                if (heroShop.CanGoToMenu())
                    interWave.UpdateShopState(UIState.Menu);
                else heroShop.CancelSelection();
                    break;
            case UIState.PartyManagement:
                partyController.Disable();
                interWave.UpdateShopState(UIState.Menu);
                break;
            default:
                interWave.UpdateShopState(UIState.Menu);
                break;
        }
    }

    public void SetActive(GameState gameState)
    {
        if (gameState == GameState.Shop)
            gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }

    [Serializable]
    private class InterWave
    {
        [Header("Buttons")]
        [SerializeField] private Button openShop;
        [SerializeField] private Button manageParty;
        [SerializeField] private Button startNextWave;
        [field: SerializeField] public Button cancelButton { get; private set; }

        [Header("UiObjects")]
        [SerializeField] private GameObject interWaveMenu;
        [SerializeField] private GameObject heroShop;
        [SerializeField] private GameObject partyManager;

        public UIState currentState { get; private set; }

        public void Initialize()
        {
            openShop.onClick.AddListener(delegate { UpdateShopState(UIState.Shop); });
            manageParty.onClick.AddListener(delegate { UpdateShopState(UIState.PartyManagement); });

            startNextWave.onClick.AddListener(StartNextWave);

            UpdateShopState(UIState.Menu);
        }

        public void Destroy()
        {
            openShop.onClick.RemoveListener(delegate { UpdateShopState(UIState.Shop); });
            manageParty.onClick.RemoveListener(delegate { UpdateShopState(UIState.PartyManagement); });

            startNextWave.onClick.RemoveListener(StartNextWave);
        }

        public void UpdateShopState(UIState newState)
        {
            switch (newState)
            {
                case UIState.Menu:
                    OpenMenu();
                    cancelButton.gameObject.SetActive(false);
                    break;
                case UIState.Shop:
                    OpenShop();
                    cancelButton.gameObject.SetActive(true);
                    break;
                case UIState.PartyManagement:
                    OpenPartyManager();
                    cancelButton.gameObject.SetActive(true);
                    break;
            }

            currentState = newState;
        }

        #region MenuOpener
        private void OpenMenu()
        {
            interWaveMenu.SetActive(true);
            heroShop.SetActive(false);
            partyManager.SetActive(false);
        }

        private void OpenShop()
        {
            interWaveMenu.SetActive(false);
            heroShop.SetActive(true);
            partyManager.SetActive(false);
        }

        private void OpenPartyManager()
        {
            interWaveMenu.SetActive(false);
            heroShop.SetActive(false);
            partyManager.SetActive(true);
        }
        #endregion

        private void StartNextWave()
        {
            GameManager.UpdateGameState(GameState.Wave);
        }
    }

    protected enum UIState
    {
        Menu,
        Shop,
        PartyManagement
    }
}