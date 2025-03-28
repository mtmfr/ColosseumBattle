using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public GameState currentGameState { get; private set; } = GameState.WaveStart;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        
        instance = this;

        DontDestroyOnLoad(gameObject);
    }
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