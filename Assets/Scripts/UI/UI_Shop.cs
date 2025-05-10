using UnityEngine;

public class UI_Shop : MonoBehaviour
{
    [SerializeField] private SO_UnitList heroList;

    public void SetActive(GameState gameState)
    {
        if (gameState == GameState.Shop)
            gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
}
