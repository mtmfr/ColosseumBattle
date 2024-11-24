using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [Header("Start UI")]
    private int currentHero;

    [SerializeField] private GameObject startUI;
    [Space]
    [SerializeField] private GameObject heroDesc;
    [SerializeField] private Image heroImage;
    [SerializeField] private TextMeshProUGUI description;
    [Space]
    [SerializeField] private GameObject HeroSelectScreen;
    [SerializeField] private Hero[] heroToSpawn = new Hero[6];
    [SerializeField] private GameObject descCancel;

    #endregion

    #region Shop Variables
    [Header("Shop UI")]
    [SerializeField] private GameObject shop;
    [SerializeField] private TextMeshProUGUI shopGoldValue;
    [SerializeField] private TextMeshProUGUI[] HeroPrice;
    #endregion

    #region GameOver Variables
    [Header("Game Over")]
    [SerializeField] private GameObject GameOverScreen;
    [SerializeField] private GameObject GameOverBG;
    [SerializeField] private float fadeDuration;

    [SerializeField] private GameObject GOButtons;

    #endregion

    #region Unity functions

    private void OnEnable()
    {
        EventManager.Instance.MiscEvent.GoldValueChange += GoldNumber;
        EventManager.Instance.MiscEvent.TimerValueChange += Timer;
        EventManager.Instance.WaveEvent.WaveStart += CurrentWave;
        EventManager.Instance.WaveEvent.OpenShop += OpenShop;
        EventManager.Instance.WaveEvent.GameOver += ShowGameOverScreen;

    }

    private void OnDisable()
    {
        EventManager.Instance.MiscEvent.GoldValueChange -= GoldNumber;
        EventManager.Instance.MiscEvent.TimerValueChange -= Timer;
        EventManager.Instance.WaveEvent.WaveStart -= CurrentWave;
        EventManager.Instance.WaveEvent.OpenShop -= OpenShop;
        EventManager.Instance.WaveEvent.GameOver -= ShowGameOverScreen;
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
        HeroSelectScreen.SetActive(false);
    }

    private void UpdateHeroDesc()
    {
        Hero hero = heroToSpawn[currentHero];
        heroDesc.GetComponentInChildren<Image>().sprite = hero.CharSprite;
        heroDesc.GetComponentInChildren<TextMeshProUGUI>().text = $"pv : {hero.MaxHealth}, att : {hero.Attack}, mag : {hero.Magic}, speed : {hero.Speed}, attSpeed : {hero.AttSpeed}";
    }

    public void SpawnSelectedHero()
    {
        //GameObject hero = Instantiate(heroToSpawn[currentHero].gameObject, WaveManager.Instance.SpawnZone(WaveManager.Instance.HeroSpawnZone), Quaternion.identity);
        GameObject hero = heroToSpawn[currentHero].gameObject;
        WaveManager.Instance.HeroList.Add(hero);
        startUI.SetActive(false);
        battleUI.SetActive(true);
        GameManager.Instance.UpdateGameState(GameState.Fight);
    }

    public void Cancel()
    {
        heroDesc.SetActive(false);
        descCancel.SetActive(false);
        HeroSelectScreen.SetActive(true);
    }
    #endregion

    #region Shop
    private void OpenShop()
    {
        shop.SetActive(true);
        battleUI.SetActive(false);

        for (int costToSet = 0; costToSet < HeroPrice.Length; costToSet++)
        {
            int heroPrice = heroToSpawn[costToSet].Cost * WaveManager.Instance.HeroList.Count;
            HeroPrice[costToSet].text = heroPrice.ToString();
        }
    }

    public void BuyNewHero(int heroIndex)
    {
        int heroCost = heroToSpawn[heroIndex].Cost;
        int nbHeroInParty = WaveManager.Instance.HeroList.Count;

        if (GameManager.Instance.Gold - heroCost * nbHeroInParty >= 0)
        {
            GameObject hero = heroToSpawn[heroIndex].gameObject;
            GameManager.Instance.Gold -= heroCost * nbHeroInParty;
            WaveManager.Instance.HeroList.Add(hero);
            EventManager.Instance.MiscEvent.OnGoldValueChange(GameManager.Instance.Gold);

            for (int costToSet = 0; costToSet < HeroPrice.Length; costToSet++)
            {
                int heroPrice = heroToSpawn[costToSet].Cost * (nbHeroInParty + 1);
                HeroPrice[costToSet].text = heroPrice.ToString();
            }
        }
        else return;
    }

    public void CloseShop()
    {
        shop.SetActive(false);
        battleUI.SetActive(true);
        GameManager.Instance.UpdateGameState(GameState.Fight);
    }
    #endregion

    #region GameOver
    private void ShowGameOverScreen()
    {
        GameOverScreen.SetActive(true);
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
        GameOverBG.SetActive(true);
        for (float alpha = 0; alpha < 1; alpha += Time.fixedDeltaTime)
        {
            GameOverBG.GetComponent<Image>().color = new Color(0, 0, 0, alpha);
        }
        yield return new WaitForSeconds(fadeDuration);
        GOButtons.SetActive(true);
    }

    #endregion
}
