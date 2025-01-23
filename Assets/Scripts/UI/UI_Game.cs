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
        GameManager.Instance.OnGameStateChanged += StartTimer;

        MiscEvent.GoldValueChange += GoldNumber;
        MiscEvent.TimerValueChange += Timer;
        WaveEvent.WaveStart += CurrentWave;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameStateChanged -= StartTimer;

        MiscEvent.GoldValueChange -= GoldNumber;
        MiscEvent.TimerValueChange -= Timer;
        WaveEvent.WaveStart -= CurrentWave;
    }

    private void GoldNumber(int gold)
    {
        battleGoldValue.text = gold.ToString();
    }

    private void StartTimer(GameState gameState)
    {
        if (gameState != GameState.Fight)
            return;

        StartCoroutine(TimerCoroutine());
    }

    private void Timer(int time)
    {
        if (GameManager.Instance.State == GameState.Fight)
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
        MiscEvent.OnTimerChange(WaveManager.Instance.TimePerWave);
    }
}
