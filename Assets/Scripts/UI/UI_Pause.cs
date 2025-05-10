using UnityEngine;

public class UI_Pause : MonoBehaviour
{
    public void SetActive(GameState gameState)
    {
        if (gameState == GameState.Pause)
            gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
}
