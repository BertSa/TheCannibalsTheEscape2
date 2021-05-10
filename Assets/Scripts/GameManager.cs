using System;
using UnityEngine;
using static GameManager.GameState;
using static SoundManager;
using static SoundManager.Ambiances;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{
    private GameState _gameState = Playing;
    private Ambiances ambiance = Followed;
    public EventGameState onGameStateChanged;
    public EventAmbiance onAmbianceChanged;

    private void Start()
    {
        var potentialExits = FindObjectsOfType<PotentialExit>();
        if (potentialExits.Length < 1) return;
        potentialExits[Random.Range(0, potentialExits.Length)].SetAsExit(true);
    }

    private void Update()
    {
        if ((Input.GetKey(KeyCode.Escape)))
        {
        }
    }


    public void SetAmbiance(Ambiances actual)
    {
        var previous = ambiance;
        ambiance = actual;
        onAmbianceChanged.Invoke(previous, actual);
    }

    public void SetGameState(GameState gameState)
    {
        var oldGameState = _gameState;
        switch (_gameState = gameState)
        {
            case Won:
                Time.timeScale = 0;
                break;
            case LostTorch:
                Time.timeScale = 0;
                if (SoundManager.IsInitialized) SoundManager.Instance.EndGame();
                break;
            case LostCannibals:
                Time.timeScale = 0;
                break;
            case Playing:
                Time.timeScale = 1;
                break;
            case Pause:
                Time.timeScale = 0;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
        }

        onGameStateChanged.Invoke(oldGameState, _gameState);
    }


    public enum GameState
    {
        Playing,
        LostCannibals,
        LostTorch,
        Won,
        Pause
    }
}