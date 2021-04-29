using System;
using UnityEngine;
using Random = UnityEngine.Random;
using static GameManager.GameState;

public class GameManager : Singleton<GameManager>
{
    private GameState _gameState = Playing;

    private void Start()
    {
        var potentialExits = FindObjectsOfType<PotentialExit>();
        if (potentialExits.Length < 1) return;
        potentialExits[Random.Range(0, potentialExits.Length)].SetAsExit(true);
    }
    
    // ReSharper disable once MemberCanBeMadeStatic.Global
    public void EndGame(EndingStatus status)
    {
        switch (status)
        {
            case EndingStatus.LostTorch:
                SetGameState(Lost);
                break;
            case EndingStatus.WinExit:
                SetGameState(Won);
                break;
            case EndingStatus.LostZombies:
                SetGameState(Lost);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }
    }


    private void SetGameState(GameState gameState)
    {
        switch (_gameState = gameState)
        {
            case Won:
                Time.timeScale = 0;
                break;
            case Lost:
                Time.timeScale = 0;
                break;
            case Playing:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
        }
    }

    public enum GameState
    {
        Lost,
        Playing,
        Won
    }

    public enum EndingStatus
    {
        LostZombies,
        LostTorch,
        WinExit
    }
}