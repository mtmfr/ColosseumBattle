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
        OnGameStateChanged += GameStateMusic;
    }

    private void OnDisable()
    {
        OnGameStateChanged += GameStateMusic;
    }

    #endregion

    private void GameStateMusic(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.MainMenu:
                mainMenuMusic.Play();
                fightMusic.Stop();
                shopMusic.Stop();
                return;
            case GameState.Fight:
                mainMenuMusic.Stop();
                fightMusic.Play();
                shopMusic.Stop();
                return;
            case GameState.Shop:
                mainMenuMusic.Stop();
                fightMusic.Stop();
                shopMusic.Play();
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
                Gold = 0;
                break;
            case GameState.Fight:
                MiscEvent.OnGoldValueChange(Gold);
                break;
            case GameState.Shop:
                WaveEvent.WaveEnded();
                WaveEvent.OpenShopEvent();
                break;
            case GameState.Lose:
                WaveEvent.GameOverEvent();
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
