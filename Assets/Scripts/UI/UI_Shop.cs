using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Shop : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI shopGoldValue;
    [SerializeField] private TextMeshProUGUI[] HeroPrice;

    [SerializeField] private List<SO_HeroStats> heroStats;
    [SerializeField] private List<Hero> heroes;

    private int heroInParty;


    private void OnEnable()
    {
        WaveEvent.OpenShop += OpenShop;
        WaveEvent.OnHeroListChanged += UpdateHeroInParty;
    }

    private void OnDisable()
    {
        WaveEvent.OpenShop -= OpenShop;
        WaveEvent.OnHeroListChanged -= UpdateHeroInParty;
    }

    private void UpdateHeroInParty(int heroInParty)
    {
        this.heroInParty = heroInParty;
    }

    private void OpenShop()
    {
        for (int costToSet = 0; costToSet < HeroPrice.Length; costToSet++)
        {
            int heroPrice = heroStats[costToSet].Cost * heroInParty;
            HeroPrice[costToSet].text = heroPrice.ToString();
        }
    }

    public void BuyNewHero(int heroIndex)
    {
        int heroCost = heroStats[heroIndex].Cost;
        int nbHeroInParty = heroInParty;

        if (GameManager.Instance.Gold - heroCost * nbHeroInParty >= 0)
        {
            GameObject hero = heroes[heroIndex].gameObject;
            GameManager.Instance.Gold -= heroCost * nbHeroInParty;
            WaveEvent.AddHeroToList(hero);
            MiscEvent.OnGoldValueChange(GameManager.Instance.Gold);

            for (int costToSet = 0; costToSet < HeroPrice.Length; costToSet++)
            {
                int heroPrice = heroStats[costToSet].Cost * (nbHeroInParty + 1);
                HeroPrice[costToSet].text = heroPrice.ToString();
            }
        }
        else return;
    }

    public void CloseShop()
    {
        GameManager.Instance.UpdateGameState(GameState.Fight);
    }
}
