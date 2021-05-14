using System;
using UnityEngine;
using UnityEngine.Events;
using static GameManager;
using static GameManager.GameState;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Cinematic cinematicBeginning;
    [SerializeField] private Cinematic cinematicEndWin;
    [SerializeField] private Cinematic cinematicEndLostTorch;
    [SerializeField] private Cinematic cinematicEndLostCannibals;
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
                cinematicBeginning.gameObject.SetActive(true);
                break;
            case Pause:
                break;
            case Playing:
                if (previous == Beginning) cinematicBeginning.gameObject.SetActive(false);
                hud.SetActive(true);
                break;
            case Won:
                cinematicEndWin.gameObject.SetActive(true);
                break;
            case LostCannibals:
                cinematicEndLostCannibals.gameObject.SetActive(true);
                break;
            case LostTorch:
                cinematicEndLostTorch.gameObject.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actual), actual, null);
        }
    }

    public void CinematicFinished(Cinematic cinematic)
    {
        if (!GameManager.IsInitialized) return;
        
        if (cinematic.Equals(cinematicBeginning))
            GameManager.Instance.SetGameState(Playing);
        else if (cinematic.Equals(cinematicEndWin))
            GameManager.Instance.SetGameState(Won);
        else if (cinematic.Equals(cinematicEndLostCannibals))
            GameManager.Instance.SetGameState(LostCannibals);
        else if (cinematic.Equals(cinematicEndLostTorch))
            GameManager.Instance.SetGameState(LostTorch);
    }
}

[Serializable]
public class EventGameState : UnityEvent<GameState, GameState>
{
}