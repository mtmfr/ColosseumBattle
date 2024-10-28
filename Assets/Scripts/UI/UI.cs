using System;
using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    #region battle UI variables
    [Header("BattleUI")]
    [SerializeField] private GameObject battleUI;
    [SerializeField] private TextMeshProUGUI battleGoldValue;
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private TextMeshProUGUI waveNumber;
    #endregion

    #region GameStart Variables
    private int heroMenu;
    [Header("Start UI")]
    private int currentHero;

    [SerializeField] private GameObject startUI;
    [Space]
    [SerializeField] private GameObject heroDesc;
    [SerializeField] private Image heroImage;
    [SerializeField] private TextMeshProUGUI description;
    [Space]
    [SerializeField] private GameObject[] HeroSelect = new GameObject[6];
    [SerializeField] private Hero[] heroToSpawn = new Hero[6];
    [SerializeField] private GameObject descCancel;

    #endregion

    [Header("Shop UI")]
    [SerializeField] private GameObject shop;
    [SerializeField] private TextMeshProUGUI shopGoldValue;
    [SerializeField] private TextMeshProUGUI HeroPrice;

    #region Unity functions

    private void OnEnable()
    {
        EventManager.Instance.MiscEvent.GoldValueChange += GoldNumber;
        EventManager.Instance.MiscEvent.TimerValueChange += Timer;
        EventManager.Instance.WaveEvent.WaveStart += CurrentWave;
        EventManager.Instance.WaveEvent.OpenShop += OpenShop;

    }

    private void OnDisable()
    {
        EventManager.Instance.MiscEvent.GoldValueChange -= GoldNumber;
        EventManager.Instance.MiscEvent.TimerValueChange -= Timer;
        EventManager.Instance.WaveEvent.WaveStart -= CurrentWave;
        EventManager.Instance.WaveEvent.OpenShop -= OpenShop;
    }
    #endregion

    #region battle UI
    private void GoldNumber(int gold)
    {
        battleGoldValue.text = gold.ToString();
        shopGoldValue.text = gold.ToString();
    }

    private void Timer(int time)
    {
        if(GameManager.Instance.State == GameState.Fight)
        {
            timer.text = time.ToString();
            WaveManager.Instance.TimePerWave--;
            StartCoroutine(TimerCoroutine());
        }
    }

    private void CurrentWave(int wave)
    {
        waveNumber.text = $"wave : {wave}";
    }

    IEnumerator TimerCoroutine()
    {
        yield return new WaitForSeconds(1);
        EventManager.Instance.MiscEvent.OnTimerChange(WaveManager.Instance.TimePerWave);
    }
    #endregion

    #region Select first hero
    public void SelectCharacter(int selectedHero)
    {
        currentHero = selectedHero;
        heroDesc.SetActive(true);
        descCancel.SetActive(true);
        UpdateHeroDesc();
        HeroSelect[currentHero].transform.parent.gameObject.SetActive(false);
    }

    private void UpdateHeroDesc()
    {
        Hero hero = heroToSpawn[currentHero];
        heroDesc.GetComponentInChildren<Image>().sprite = hero.CharSprite;
        heroDesc.GetComponentInChildren<TextMeshProUGUI>().text = $"att : {hero.attack}, mag : {hero.magic}, speed : {hero.speed}, attSpeed : {hero.attSpeed}";
    }

    public void SpawnSelectedHero()
    {
        GameManager.Instance.UpdateGameState(GameState.Fight);
        Hero heroObject = Instantiate(heroToSpawn[currentHero], WaveManager.Instance.SpawnZone(WaveManager.Instance.HeroSpawnZone), Quaternion.identity);
        GameManager.Instance.HeroList.Add(heroToSpawn[currentHero].gameObject);
        startUI.SetActive(false);
        battleUI.SetActive(true);
    }

    public void Cancel()
    {
        heroDesc.SetActive(false);
        descCancel.SetActive(false);
        HeroSelect[heroMenu].transform.parent.gameObject.SetActive(true);
    }
    #endregion

    #region Shop
    private void OpenShop()
    {
        shop.SetActive(true);
        battleUI.SetActive(false);
    }

    public void BuyNewHero(int heroIndex)
    {
        HeroPrice.text = heroToSpawn[heroIndex].cost.ToString();
        EventManager.Instance.MiscEvent.OnGoldValueChange(heroToSpawn[heroIndex].cost);
    }

    public void SellHero()
    {
        //TODO sell useless hero
    }

    public void CloseShop()
    {
        shop.SetActive(false);
        battleUI.SetActive(true);
        GameManager.Instance.UpdateGameState(GameState.Fight);
    }
    #endregion
}
