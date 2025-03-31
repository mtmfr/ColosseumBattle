using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public int gold { get; private set; }

    public GameState currentGameState { get; private set; } = GameState.WaveStart;

    private GameManager() { }

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        
        instance = this;

        DontDestroyOnLoad(gameObject); 
    }

    public event Action<GameState> OnGameStateChange;
    public void UpdateGameState(GameState gameState)
    {
        currentGameState = gameState;
        
        OnGameStateChange?.Invoke(gameState);

        switch (gameState)
        {
            case GameState.MainMenu:
                break;
            case GameState.WaveStart:
                UpdateGameState(GameState.Start);
                break;
            case GameState.Start:
                break;
            case GameState.Shop:
                break;
            case GameState.GameOver:
                break;
            default:
                break;
        }
    }

    public void AddGold(int goldToAdd) => gold += goldToAdd;
    public void RemoveGold(int goldToRemove) => gold -= goldToRemove;
}

public enum GameState
{
    MainMenu,
    Start,
    WaveStart,
    Wave,
    Shop,
    GameOver
}