using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
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
    private int index;
    [Header("Start UI")]
    [SerializeField] private GameObject startUI;
    [SerializeField] private GameObject[] HeroSelect = new GameObject[6];
    [SerializeField] private GameObject[] heroDesc = new GameObject[6];
    [SerializeField] private Hero[] heroToSpawn = new Hero[6];
    [SerializeField] private GameObject descCancel;
    #endregion

    #region Unity functions
    private void Start()
    {
        
    }
    private void OnEnable()
    {
        EventManager.Instance.MiscEvent.GoldValueChange += GoldNumber;
        EventManager.Instance.MiscEvent.TimerValueChange += Timer;
        EventManager.Instance.MiscEvent.WaveStart += CurrentWave;
    }

    private void OnDisable()
    {
        EventManager.Instance.MiscEvent.GoldValueChange -= GoldNumber;
        EventManager.Instance.MiscEvent.TimerValueChange -= Timer;
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
            GameManager.Instance.TimePerWave--;
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
        EventManager.Instance.MiscEvent.OnTimerChange(GameManager.Instance.TimePerWave);
    }
    #endregion

    #region Select first hero
    public void SelectCharacter(int i)
    {
        index = i;
        heroDesc[i].SetActive(true);
        descCancel.SetActive(true);
        HeroSelect[i].transform.parent.gameObject.SetActive(false);
    }

    private Vector2 SpawnZone(GameObject spawnZone)
    {
        var range = spawnZone.GetComponent<Collider2D>();

        var spawnPoint = new Vector2(
            Random.Range(range.bounds.min.x, range.bounds.max.x),
            Random.Range(range.bounds.min.y, range.bounds.max.y));
        return spawnPoint;
    }

    public void SpawnSelectedHero(int i)
    {
        GameManager.Instance.UpdateGameState(GameState.Fight);
        var heroObject = Instantiate(heroToSpawn[i],SpawnZone(GameManager.Instance.HeroSpawnZone), Quaternion.identity);
        GameManager.Instance.HeroList.Add(heroObject.gameObject);
        startUI.SetActive(false);
        battleUI.SetActive(true);
    }

    public void Cancel()
    {
        heroDesc[index].SetActive(false);
        descCancel.SetActive(false);
        HeroSelect[index].transform.parent.gameObject.SetActive(true);
    }
    #endregion
}
