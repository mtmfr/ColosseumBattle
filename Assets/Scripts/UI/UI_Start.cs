using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Start : MonoBehaviour
{
    /*
    [SerializeField] private GameObject heroSelectScreen;
    [SerializeField] private GameObject heroRecruitScreen;

    [SerializeField] private GameObject heroDesc;


    [SerializeField] private List<Old_SO_HeroStats> heroStats;
    [SerializeField] private List<Old_Hero> heroes;

    private int selectedHeroId;

    /// <summary>
    /// Get the selected hero id and show the heroDesc screen
    /// </summary>
    /// <param name="HeroId">the id of the selected hero</param>
    public void SelectCharacter(int HeroId)
    {
        selectedHeroId = HeroId;
        heroDesc.SetActive(true);
        heroRecruitScreen.SetActive(true);
        UpdateHeroDesc();
        heroSelectScreen.SetActive(false);
    }

    /// <summary>
    /// Update the description of stats to be ehe one of the selected hero
    /// </summary>
    private void UpdateHeroDesc()
    {
        Old_SO_HeroStats stats = heroStats[selectedHeroId];
        heroDesc.GetComponentInChildren<Image>().sprite = stats.CharIcon;
        heroDesc.GetComponentInChildren<TextMeshProUGUI>().text = $"pv : {stats.Health}, att : {stats.Attack}, mag : {stats.Magic}, speed : {stats.Speed}, attSpeed : {stats.AttSpeed}";
    }

    /// <summary>
    /// Spawn the selected hero
    /// </summary>
    public void SpawnSelectedHero()
    {
        //GameObject hero = Instantiate(heroToSpawn[currentHero].gameObject, WaveManager.Instance.SpawnZone(WaveManager.Instance.heroSpawnZone), Quaternion.identity);
        GameObject hero = heroes[selectedHeroId].gameObject;
        WaveEvent.AddHeroToList(hero);
        Old_GameManager.Instance.UpdateGameState(Old_GameState.Fight);
    }

    /// <summary>
    /// Go back to the hero selection screen
    /// </summary>
    public void Cancel()
    {
        heroDesc.SetActive(false);
        heroRecruitScreen.SetActive(false);
        heroSelectScreen.SetActive(true);
    }
    */
}
