using System;
using System.Collections.Generic;
using UnityEngine;

public class Old_GameManager : MonoBehaviour
{
    public static Old_GameManager Instance { get; private set; }


    #region Gamestate
    public Old_GameState State { get; private set; }

    public event Action<Old_GameState> OnGameStateChanged;
    #endregion

    public int Gold { get; set; } = 0;

    [SerializeField] private AudioSource mainMenuMusic;
    [SerializeField] private AudioSource fightMusic;
    [SerializeField] private AudioSource shopMusic;


    #region UnityFunction
    private void Awake()
    {
        if(Instance)
            Destroy(gameObject);
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        UpdateGameState(Old_GameState.MainMenu);
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

    private void GameStateMusic(Old_GameState gameState)
    {
        switch (gameState)
        {
            case Old_GameState.MainMenu:
                mainMenuMusic.Play();
                fightMusic.Stop();
                shopMusic.Stop();
                return;
            case Old_GameState.Fight:
                mainMenuMusic.Stop();
                fightMusic.Play();
                shopMusic.Stop();
                return;
            case Old_GameState.Shop:
                mainMenuMusic.Stop();
                fightMusic.Stop();
                shopMusic.Play();
                return;
        }
    }

    public void UpdateGameState(Old_GameState newState)
    {
        State = newState;

        switch(State)
        {
            case Old_GameState.MainMenu:
                break;
            case Old_GameState.Start:
                Gold = 0;
                break;
            case Old_GameState.Fight:
                MiscEvent.OnGoldValueChange(Gold);
                break;
            case Old_GameState.Shop:
                WaveEvent.WaveEnded();
                WaveEvent.OpenShopEvent();
                break;
            case Old_GameState.Lose:
                WaveEvent.GameOverEvent();
                break;
        }
        OnGameStateChanged?.Invoke(State);
    }
}

public enum Old_GameState
{
    MainMenu,
    Start,
    Fight,
    Shop,
    Lose,
}
