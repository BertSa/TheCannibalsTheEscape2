using System;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject cinematic;
    [SerializeField] private GameObject hud;

    private void Start()
    {
        GameManager.Instance.onGameStateChanged.AddListener(HandleGameStateChanged);
    }

    private void HandleGameStateChanged(GameManager.GameState previous, GameManager.GameState actual)
    {
        switch (actual)
        {
            case GameManager.GameState.Playing:
                hud.SetActive(true);
                break;
            case GameManager.GameState.Won:
                cinematic.SetActive(false);
                break;
            case GameManager.GameState.LostCannibals:
                cinematic.SetActive(false);
                break;
            case GameManager.GameState.LostTorch:
                cinematic.SetActive(false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actual), actual, null);
        }
    }
}

[Serializable]
public class EventGameState : UnityEvent<GameManager.GameState, GameManager.GameState>
{
}