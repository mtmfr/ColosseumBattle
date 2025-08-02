using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class HeroShop
{
    [field: Header("ShopObject")]
    [SerializeField] private GameObject heroSelection;
    [SerializeField] private GameObject heroDescription;

    [field: Header("Selection")]
    [SerializeField] private SO_UnitList heroList;
    [SerializeField] private Button[] heroButtons;

    [Header("Description")]
    [SerializeField] private Image selectedImage;
    [SerializeField] private TextMeshProUGUI selectedDescription;
    [SerializeField] private TextMeshProUGUI goldAmount;
    [SerializeField] private Button BuyHero;

    [Header("Feedback")]
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Color startingColor;
    [SerializeField] private string goldLacking;
    [SerializeField] private string partyFull;
    [SerializeField] private float fadeDuration;

    private Hero selectedHero;

    private bool isHeroSelected = false;

    public event Action HeroBought;

    #region Object Init/destruction
    public void Initialize()
    {
        BindHeroesButtons();

        BuyHero.onClick.AddListener(delegate { BuySelectedHero(selectedHero); });
    }

    public void Enable()
    {
        UpdateGoldText();
    }

    public void Destroy()
    {
        UnBindHeroesButton();

        BuyHero.onClick.RemoveListener(delegate { BuySelectedHero(selectedHero); });
    }
    #endregion

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
        for (int button = 0; button < heroButtons.Length; button++)
        {
            Button heroButton = heroButtons[button];

            //create an id variable so that every hero can be selected
            //without it the only called hero will be the one with the last id
            int heroId = button;
            heroButton.onClick.RemoveListener(delegate { OnHeroSelected(heroId); });
        }
    }
    #endregion

    #region HeroDescription
    private void OnHeroSelected(int id)
    {
        //Check if hero can be bought
        if (!PartyManager.CanHeroBeBought())
        {
            FeedBackFade(partyFull);
            return;
        }

        Unit selectedUnit = heroList.units[id];

        if (selectedUnit is not Hero)
        {
            Debug.LogError("The selected unit is not a hero");
            return;
        }

        isHeroSelected = true;

        selectedHero = selectedUnit as Hero;

        UpdateDescription(selectedHero);

        heroSelection.SetActive(false);
        heroDescription.SetActive(true);
    }

    /// <summary>
    /// Update the description to match the one pf the selected hero
    /// </summary>
    /// <param name="selectedHero">The hero that was selected</param>
    private void UpdateDescription(Hero selectedHero)
    {
        UIParameters uiInfo = selectedHero.heroSO.uiParameters;

        selectedImage.sprite = uiInfo.characterSprite;
        selectedDescription.text = uiInfo.heroDescription;
    }

    private void BuySelectedHero(Hero hero)
    {
        if (hero == null)
        {
            Debug.LogError("The value of the hero to buy is null");
            return;
        }

        int heroCost = hero.heroSO.miscParameters.cost;

        if (GameManager.gold - heroCost < 0)
        {
            FeedBackFade(goldLacking);
        }
        else
        {
            PartyManager.NewHeroBought(hero);

            GameManager.RemoveGold(heroCost);

            HeroBought?.Invoke();

            UpdateGoldText();
            CancelSelection();
        }
    }

    #endregion

    /// <summary>
    /// Get wether or not the ui state can go to the the menu
    /// </summary>
    /// <returns>true, if no hero has been selected.
    /// <br>false, if a hero has been selected</br>
    /// </returns>
    public bool CanGoToMenu() => !isHeroSelected;

    public void CancelSelection()
    {
        selectedHero = null;
        isHeroSelected = false;

        heroSelection.SetActive(true);
        heroDescription.SetActive(false);
    }

    private void UpdateGoldText()
    {
        goldAmount.text = GameManager.gold.ToString();
    }

    private async void FeedBackFade(string textToshow)
    {
        float remainingTime = fadeDuration;

        float fadeRatio = remainingTime / fadeDuration;

        Color currentColor = startingColor;

        feedbackText.text = textToshow;
        feedbackText.color = currentColor;

        while (fadeRatio > 0.01f)
        {
            remainingTime -= Time.deltaTime;

            fadeRatio = remainingTime / fadeDuration;

            currentColor.a = fadeRatio;
            feedbackText.color = currentColor;

            await Task.Yield();
        }
        feedbackText.color = Color.clear;
    }
}
