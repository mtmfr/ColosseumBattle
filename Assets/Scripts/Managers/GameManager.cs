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

    [field: SerializeField] public List<GameObject> HeroList { get; private set; } = new List<GameObject>();
    [field: SerializeField] public int HeroLeftInParty { get; set; }
    public List<Enemy> EnemyList { get; private set; } = new List<Enemy>();

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
    #endregion

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch(State)
        {
            case GameState.MainMenu:
                break;
            case GameState.Start:
                Gold = 0;
                WaveManager.Instance.CurrentWave = 0;
                break;
            case GameState.Fight:
                WaveManager.Instance.StartSpawn();
                WaveManager.Instance.FightStart();
                EventManager.Instance.MiscEvent.OnGoldValueChange(Instance.Gold);

                break;
            case GameState.Shop: 
                WaveManager.Instance.EndWave();
                EventManager.Instance.WaveEvent.OpenShopEvent();
                break;
            case GameState.Lose:

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
