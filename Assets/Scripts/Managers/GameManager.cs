using System;
using UnityEngine;

public static class GameManager
{
    public static int gold { get; private set; }

    public static GameState currentGameState { get; private set; } = GameState.Wave;

    public static event Action<GameState> OnGameStateChange;
    public static void UpdateGameState(GameState gameState)
    {
        currentGameState = gameState;
        
        OnGameStateChange?.Invoke(gameState);

        WaveManager waveManager = WaveManager.instance;

        switch (gameState)
        {
            case GameState.MainMenu:
                break;
            case GameState.Start:

                waveManager.GenerateNextWave();
                break;
            case GameState.Wave:
                waveManager.SwitchWaveState(WaveManager.WaveState.Start);
                break;
            case GameState.Shop:
                break;
            case GameState.GameOver:
                waveManager.ResetWave();
                break;
            default:
                break;
        }
    }

    public static void AddGold(int goldToAdd) => gold += goldToAdd;
    public static void RemoveGold(int goldToRemove) => gold -= goldToRemove;
}

public enum GameState
{
    MainMenu,
    Start,
    Wave,
    Shop,
    GameOver
}