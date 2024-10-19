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
    [SerializeField] private TextMeshProUGUI goldValue;
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private TextMeshProUGUI waveNumber;
    #endregion

    #region GameStart Variables
    private int heroMenu;
    [Header("Start UI")]
    [SerializeField] private GameObject startUI;
    [SerializeField] private GameObject[] HeroSelect = new GameObject[6];
    [SerializeField] private GameObject[] heroDesc = new GameObject[6];
    [SerializeField] private Hero[] heroToSpawn = new Hero[6];
    [SerializeField] private GameObject descCancel;
    #endregion

    [SerializeField] private GameObject shop; 

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
        goldValue.text = gold.ToString();
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
        waveNumber.text = "wave : " + wave.ToString();
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
        heroMenu = selectedHero;
        heroDesc[selectedHero].SetActive(true);
        descCancel.SetActive(true);
        HeroSelect[selectedHero].transform.parent.gameObject.SetActive(false);
    }

    public void SpawnSelectedHero(int heroIndex)
    {
        GameManager.Instance.UpdateGameState(GameState.Fight);
        Hero heroObject = Instantiate(heroToSpawn[heroIndex], WaveManager.Instance.SpawnZone(WaveManager.Instance.HeroSpawnZone), Quaternion.identity);
        GameManager.Instance.HeroList.Add(heroObject.gameObject);
        startUI.SetActive(false);
        battleUI.SetActive(true);
    }

    public void Cancel()
    {
        heroDesc[heroMenu].SetActive(false);
        descCancel.SetActive(false);
        HeroSelect[heroMenu].transform.parent.gameObject.SetActive(true);
    }
    #endregion

    private void OpenShop()
    {
        shop.SetActive(true);
    }

    public void BuyNewHero(int heroIndex)
    {

    }

    public void CloseShop()
    {
        shop.SetActive(false);
    }
}
