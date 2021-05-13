using System;
using UnityEngine;
using static GameManager.GameState;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{
    private GameState _gameState = Beginning;
    [HideInInspector] public EventGameState onGameStateChanged;


    private void Start()
    {
        var potentialExits = FindObjectsOfType<PotentialExit>();
        if (potentialExits.Length < 1) return;
        potentialExits[Random.Range(0, potentialExits.Length)].SetAsExit(true);
    }

    private void Update()
    {
        
    }

    public void SetGameState(GameState gameState)
    {
        var oldGameState = _gameState;
        switch (_gameState = gameState)
        {
            case Beginning:
                break;
            case Pause:
                Time.timeScale = 0;
                break;
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
            default:
                throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
        }

        onGameStateChanged.Invoke(oldGameState, _gameState);
    }


    public enum GameState
    {
        Playing,

        /// <summary>
        /// quand le joueur se fait manger par les cannibals...
        /// </summary>
        LostCannibals,

        /// <summary>
        /// quand la flamme du joueur s'éteinds...
        /// </summary>
        LostTorch,

        /// <summary>
        /// quand la partie est gagné
        /// </summary>
        Won,

        /// <summary>
        /// quand la partie est en pause
        /// </summary>
        Pause,
        Beginning
    }
}