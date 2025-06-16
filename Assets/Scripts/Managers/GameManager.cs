using System;
using UnityEngine.SceneManagement;

public static class GameManager
{
    public static int gold { get; private set; }

    public static GameState currentGameState { get; private set; } = GameState.None;

    public static event Action<GameState> OnGameStateChange;
    public static void UpdateGameState(GameState gameState)
    {

        WaveManager waveManager = WaveManager.instance;

        switch (gameState)
        {
            case GameState.MainMenu:
                break;
            case GameState.Start:
                waveManager.GenerateNextWave();
                break;
            case GameState.Wave:
                if (currentGameState == GameState.Pause)
                    waveManager.UpdateWaveState(WaveManager.WaveState.Middle);
                else waveManager.UpdateWaveState(WaveManager.WaveState.Start);
                break;
            case GameState.Shop:
                break;
            case GameState.GameOver:
                ObjectPool.DiscardAllObject();
                waveManager.ResetWaveCount();
                break;
            default:
                break;
        }
        
        OnGameStateChange?.Invoke(gameState);
        currentGameState = gameState;
    }

    public static void ResetScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        GC.Collect();
        SceneManager.LoadScene(currentScene.buildIndex);
        UpdateGameState(GameState.MainMenu);
    }

    public static void AddGold(int goldToAdd)
    {
        if (gold + goldToAdd <= 9999)
            gold += goldToAdd;
        else gold = 9999;
    } 
    public static void RemoveGold(int goldToRemove) => gold -= goldToRemove;
}

public enum GameState
{
    None,
    MainMenu,
    Start,
    Wave,
    Shop,
    Pause,
    GameOver
}