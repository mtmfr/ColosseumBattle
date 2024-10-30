using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region instance creation
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("GameManager is null");
            }
            return _instance;
        }
    }
    #endregion


    #region Gamestate
    public GameState State { get; private set; }

    public event Action<GameState> OnGameStateChanged;
    #endregion

    public int Gold { get; set; } = 0;

    [SerializeField] private AudioSource mainMenuMusic;
    [SerializeField] private AudioSource fightMusic;
    [SerializeField] private AudioSource shopMusic;


    #region UnityFunction
    private void Awake()
    {
        if(_instance)
            Destroy(gameObject);
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        UpdateGameState(GameState.MainMenu);
    }

    private void OnEnable()
    {
        Instance.OnGameStateChanged += GameStateMusic;
    }

    private void OnDisable()
    {
        Instance.OnGameStateChanged += GameStateMusic;
    }

    #endregion

    private void GameStateMusic(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.MainMenu:
                Instance.mainMenuMusic.Play();
                Instance.fightMusic.Stop();
                Instance.shopMusic.Stop();
                return;
            case GameState.Fight:
                Instance.mainMenuMusic.Stop();
                Instance.fightMusic.Play();
                Instance.shopMusic.Stop();
                return;
            case GameState.Shop:
                Instance.mainMenuMusic.Stop();
                Instance.fightMusic.Stop();
                Instance.shopMusic.Play();
                return;
        }
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch(State)
        {
            case GameState.MainMenu:
                break;
            case GameState.Start:
                Instance.Gold = 0;
                WaveManager.Instance.CurrentWave = 0;
                break;
            case GameState.Fight:
                WaveManager.Instance.FightStart();
                WaveManager.Instance.StartHeroSpawn();
                WaveManager.Instance.GenerateWave();
                EventManager.Instance.MiscEvent.OnGoldValueChange(Instance.Gold);
                break;
            case GameState.Shop: 
                WaveManager.Instance.EndWave();
                EventManager.Instance.WaveEvent.OpenShopEvent();
                break;
            case GameState.Lose:
                EventManager.Instance.WaveEvent.GameOverEvent();
                break;
        }
        OnGameStateChanged?.Invoke(State);
    }
}

public enum GameState
{
    MainMenu,
    Start,
    Fight,
    Shop,
    Lose,
}
