using System;
using UnityEngine;
using UnityEngine.Events;
using static GameManager.GameState;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private GameObject cinematic;
    [SerializeField] private GameObject hud;

    private void Start()
    {
        if (GameManager.IsInitialized) GameManager.Instance.onGameStateChanged.AddListener(HandleGameStateChanged);
    }

    private void HandleGameStateChanged(GameManager.GameState previous, GameManager.GameState actual)
    {
        switch (actual)
        {
            case Beginning:
                
                break;
            case Pause:
                break;
            case Playing:
                hud.SetActive(true);
                break;
            case Won:
                cinematic.SetActive(true);
                break;
            case LostCannibals:
                cinematic.SetActive(true);
                break;
            case LostTorch:
                cinematic.SetActive(true);
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