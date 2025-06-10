using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_AfterWave : MonoBehaviour
{
    [SerializeField] private AfterWave afterWave;
    [SerializeField] private HeroShop heroShop;
    [SerializeField] private PartyController partyController;

    private void Start()
    {
        afterWave.Initialize();
        heroShop.Initialize();

        heroShop.CloseShop += OnShopCancel;
    }

    private void OnDestroy()
    {
        afterWave.Destroy();
        heroShop.Destroy();

        heroShop.CloseShop -= OnShopCancel;
    }

    private void OnShopCancel()
    {
        afterWave.CloseShop();
    }

    public void SetActive(GameState gameState)
    {
        if (gameState == GameState.Shop)
            gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }

    [Serializable]
    private class AfterWave
    {
        [Header("Buttons")]
        [SerializeField] private Button openShop;
        [SerializeField] private Button manageParty;
        [SerializeField] private Button startNextWave;

        [Header("UiObjects")]
        [SerializeField] private GameObject shopMenu;
        [SerializeField] private GameObject heroShop;
        [SerializeField] private GameObject partyManager;

        public void Initialize()
        {
            openShop.onClick.AddListener(OpenShop);
            manageParty.onClick.AddListener(OpenPartyManager);
        }

        public void Destroy()
        {
            openShop.onClick.RemoveListener(OpenShop);
            manageParty.onClick.RemoveListener(OpenPartyManager);
        }

        private void OpenShop()
        {
            shopMenu.SetActive(false);
            heroShop.SetActive(true);
        }

        public void CloseShop()
        {
            shopMenu.SetActive(true);
            heroShop.SetActive(false);
        }

        private void OpenPartyManager()
        {
            shopMenu.SetActive(false);
            partyManager.SetActive(true);
        }

        public void ClosePartyManager()
        {
            shopMenu.SetActive(true);
            partyManager.SetActive(false);
        }
    }

    [Serializable]
    private class HeroShop
    {
        private ShopState currentshopState;

        public event Action CloseShop;

        [field: Header("ShopObject")]
        [SerializeField] private GameObject heroSelection;
        [SerializeField] private GameObject heroDescription;

        [field: Header("Selection")]
        [SerializeField] private SO_UnitList heroList;
        [SerializeField] private Button[] heroButtons;

        [Header("Description")]
        [SerializeField] private Image selectedImage;
        [SerializeField] private TextMeshProUGUI selectedDescription;
        [SerializeField] private Button BuyHero;

        [Header("Cancel")]
        [SerializeField] private Button cancel;

        public void Initialize()
        {
            UpdateShopState(ShopState.Selection);
            BindHeroesButtons();

            cancel.onClick.AddListener(Cancel);
        }

        public void Destroy()
        {
            UnBindHeroesButton();
            cancel.onClick.RemoveListener(Cancel);
        }

        #region HeroSelection
        private void BindHeroesButtons()
        {
            if (heroButtons.Length != heroList.units.Length)
            {
                string amount = heroButtons.Length > heroList.units.Length ? "greater" : "lower";

                Debug.LogError($"The amount of buttons is {amount} than the number of heroes");
                return;
            }

            //Bind SelectHero to the delegate of the buttons in heroButtons
            for (int button = 0; button < heroButtons.Length; button++)
            {
                Button heroButton = heroButtons[button];

                if (heroButton == null)
                    Debug.LogError($"No button at id {button}", heroButton);

                //create an id variable so that every hero can be selected
                //without it the only called hero will be the one with the last id
                int heroId = button;
                heroButton.onClick.AddListener(delegate { OnHeroSelected(heroId); });
            }
        }

        private void UnBindHeroesButton()
        {
            for (int id = 0; id < heroButtons.Length; id++)
            {
                Button heroButton = heroButtons[id];

                heroButton.onClick.RemoveAllListeners();
            }
        }
        #endregion

        #region HeroDescription
        private void OnHeroSelected(int id)
        {
            //Check if hero can be bought

            if (!PartyManager.CanHeroBeBought())
            {
                //Todo add feedback to show that party is full
                return;
            }

            Unit selectedUnit = heroList.units[id];

            if (selectedUnit is not Hero)
            {
                Debug.LogError("The selected unit is not a hero");
                return;
            }

            UpdateShopState(ShopState.Description);

            Hero selectedHero = selectedUnit as Hero;

            int heroCost = selectedHero.heroSO.miscParameters.cost;

            if (GameManager.gold - heroCost < 0)
            {
                //Todo Add feedback To show that buying is not possible
            }
            else
            {
                //Add heroTo Party
                PartyManager.NewHeroBought(selectedHero);
                //Remove the cost of gold
                GameManager.RemoveGold(heroCost);
            }
        }

        #endregion

        private void UpdateShopState(ShopState newShopState)
        {
            switch (newShopState)
            {
                case ShopState.Selection:
                    heroSelection.SetActive(true);
                    heroDescription.SetActive(false);
                    break;
                case ShopState.Description:
                    heroSelection.SetActive(false);
                    heroDescription.SetActive(true);
                    break;
            }
            currentshopState = newShopState;
        }

        private void Cancel()
        {
            switch (currentshopState)
            {
                case ShopState.Selection:
                    CloseShop.Invoke();
                    break;
                case ShopState.Description:
                    UpdateShopState(ShopState.Selection);
                    break;
            }
        }

        private enum ShopState : byte
        {
            Selection,
            Description
        }
    }

    [Serializable]
    private class PartyController
    {
        
    }
}