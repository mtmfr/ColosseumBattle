using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] private AudioSource fightMusic;
    [SerializeField] private AudioSource shopMusic;

    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += ChangeCurrentMusic;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameStateChanged -= ChangeCurrentMusic;
    }

    private void ChangeCurrentMusic(GameState gameState)
    {
        bool PlayFightMusic = gameState switch
        {
            GameState.Fight => true,
            GameState.Shop => false,
            _ => false,
        };

        if (PlayFightMusic)
        {
            fightMusic.Play();
            shopMusic.Stop();
        }
        else
        {
            fightMusic.Stop();
            shopMusic.Play();
        }
    }
}
