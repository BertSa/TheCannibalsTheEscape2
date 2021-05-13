using System;
using UnityEngine;
using UnityEngine.Events;
using static GameManager;
using static GameManager.GameState;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private GameObject cinematicBeginning;
    [SerializeField] private GameObject cinematicEndWin;
    [SerializeField] private GameObject cinematicEndLostTorch;
    [SerializeField] private GameObject cinematicEndLostCannibals;
    [SerializeField] private GameObject hud;

    private void Start()
    {
        if (GameManager.IsInitialized) GameManager.Instance.onGameStateChanged.AddListener(HandleGameStateChanged);
        HandleGameStateChanged(Beginning, Beginning);
    }

    private void HandleGameStateChanged(GameState previous, GameState actual)
    {
        switch (actual)
        {
            case Beginning:
                cinematicBeginning.SetActive(true);
                break;
            case Pause:
                break;
            case Playing:
                if (previous == Beginning) cinematicBeginning.SetActive(false);
                hud.SetActive(true);
                break;
            case Won:
                cinematicEndWin.SetActive(true);
                break;
            case LostCannibals:
                cinematicEndLostCannibals.SetActive(true);
                break;
            case LostTorch:
                cinematicEndLostTorch.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actual), actual, null);
        }
    }
}

[Serializable]
public class EventGameState : UnityEvent<GameState, GameState>
{
}