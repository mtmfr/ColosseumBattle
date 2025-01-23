using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Start : MonoBehaviour
{
    [SerializeField] private GameObject HeroSelectScreen;
    [SerializeField] private GameObject descCancel;

    [SerializeField] private GameObject heroDesc;


    [SerializeField] private List<SO_HeroStats> heroStats;
    [SerializeField] private List<Hero> heroes;

    private int selectedHeroId;

    // Start is called before the first frame update
    void Start()
    {

    }
    public void SelectCharacter(int HeroId)
    {
        selectedHeroId = HeroId;
        heroDesc.SetActive(true);
        descCancel.SetActive(true);
        UpdateHeroDesc();
        HeroSelectScreen.SetActive(false);
    }

    private void UpdateHeroDesc()
    {
        SO_HeroStats stats = heroStats[selectedHeroId];
        heroDesc.GetComponentInChildren<Image>().sprite = stats.CharIcon;
        heroDesc.GetComponentInChildren<TextMeshProUGUI>().text = $"pv : {stats.Health}, att : {stats.Attack}, mag : {stats.Magic}, speed : {stats.Speed}, attSpeed : {stats.AttSpeed}";
    }

    public void SpawnSelectedHero()
    {
        //GameObject hero = Instantiate(heroToSpawn[currentHero].gameObject, WaveManager.Instance.SpawnZone(WaveManager.Instance.heroSpawnZone), Quaternion.identity);
        GameObject hero = heroes[selectedHeroId].gameObject;
        WaveEvent.AddHeroToList(hero);
        GameManager.Instance.UpdateGameState(GameState.Fight);
    }

    public void Cancel()
    {
        heroDesc.SetActive(false);
        descCancel.SetActive(false);
        HeroSelectScreen.SetActive(true);
    }
}
