using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private int baseRoundDuration;
    private int roundDuration;

    private Old_GameState previousGameState;

    private void OnEnable()
    {
        Old_GameManager.Instance.OnGameStateChanged += SetTimer;
    }

    private void OnDisable()
    {
        Old_GameManager.Instance.OnGameStateChanged -= SetTimer;
    }

    private void SetTimer(Old_GameState gameState)
    {
        if (previousGameState == Old_GameState.Shop)
            roundDuration = baseRoundDuration;

        if (gameState != Old_GameState.Fight)
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
