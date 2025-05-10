using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Game : MonoBehaviour
{
    [SerializeField] private Button pauseButton;

    [SerializeField] private TextMeshProUGUI timerText;
    private Timer timer;

    private float passedTime;

    private GameState lastState;
    private void OnEnable()
    {
        if (lastState != GameState.Pause)
        {
            timer = new(timerText);
            passedTime = 0;
        }
    }

    private void Start()
    {
        pauseButton.onClick.AddListener(PauseGame);
    }

    private void Update()
    {
        if (GameManager.currentGameState != GameState.Wave)
            return;

        if (WaveManager.instance.waveState != WaveManager.WaveState.Middle)
            return;

        if (timer == null)
            return;

        passedTime += Time.deltaTime;
        timer.UpdateTime(passedTime);
    }

    private void PauseGame() => GameManager.UpdateGameState(GameState.Pause);

    public void SetActive(GameState gameState)
    {
        if (gameState == GameState.Wave)
            gameObject.SetActive(true);
        else gameObject.SetActive(false);

        lastState = gameState;
    }
}

public class Timer
{
    private TextMeshProUGUI timerText;

    public Timer(TextMeshProUGUI timerText)
    {
        this.timerText = timerText;
    }

    public void UpdateTime(float time)
    {
        int passedTimeAsInt = Mathf.CeilToInt(time);

        int seconds = passedTimeAsInt % 60;

        int minutes = passedTimeAsInt / 60;

        string secondsText = seconds < 10 ? $"0{seconds}" : seconds.ToString();

        string minutesText = minutes < 10 ? $"0{minutes}" : minutes.ToString();

        timerText.text = minutesText + ":" + secondsText;
    }
}
