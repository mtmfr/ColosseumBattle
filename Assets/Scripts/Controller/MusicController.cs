using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] private AudioSource fightMusic;
    [SerializeField] private AudioSource shopMusic;

    private void OnEnable()
    {
        Old_GameManager.Instance.OnGameStateChanged += ChangeCurrentMusic;
    }

    private void OnDisable()
    {
        Old_GameManager.Instance.OnGameStateChanged -= ChangeCurrentMusic;
    }

    private void ChangeCurrentMusic(Old_GameState gameState)
    {
        bool PlayFightMusic = gameState switch
        {
            Old_GameState.Fight => true,
            Old_GameState.Shop => false,
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
