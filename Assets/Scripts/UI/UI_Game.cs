using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Game : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI battleGoldValue;
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private TextMeshProUGUI waveNumber;

    private void OnEnable()
    {
        MiscEvent.GoldValueChange += GoldNumber;
        MiscEvent.OnTimerValueChange += Timer;
        WaveEvent.WaveStart += CurrentWave;
    }

    private void OnDisable()
    {
        MiscEvent.GoldValueChange -= GoldNumber;
        MiscEvent.OnTimerValueChange -= Timer;
        WaveEvent.WaveStart -= CurrentWave;
    }

    private void GoldNumber(int gold)
    {
        battleGoldValue.text = gold.ToString();
    }

    private void Timer(int time)
    {
        if (Old_GameManager.Instance.State != Old_GameState.Fight)
            return;

        timer.text = time.ToString();
    }

    private void CurrentWave(int wave)
    {
        waveNumber.text = $"wave : {wave}";
    }
}
