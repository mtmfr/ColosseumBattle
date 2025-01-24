using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private int baseRoundDuration;
    private int roundDuration;

    private GameState previousGameState;

    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += SetTimer;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameStateChanged -= SetTimer;
    }

    private void SetTimer(GameState gameState)
    {
        if (previousGameState == GameState.Shop)
            roundDuration = baseRoundDuration;

        if (gameState != GameState.Fight)
            StopCoroutine(RunTimer());
        else StartCoroutine(RunTimer());

        previousGameState = gameState;
    }

    private IEnumerator RunTimer()
    {
        yield return new WaitForSeconds(roundDuration);
        roundDuration--;
        MiscEvent.TimerValueChange(roundDuration);

        if (roundDuration <= 0)
            MiscEvent.TimerFinished();
    }
}
