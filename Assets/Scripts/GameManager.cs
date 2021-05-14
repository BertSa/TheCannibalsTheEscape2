using System;
using UnityEngine;
using static GameManager.GameState;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector] public GameState gameState = Beginning;
    [HideInInspector] public EventGameState onGameStateChanged;
    protected override void Awake()
    {
        base.Awake();
        Time.timeScale = 0;
    }

    private void Start()
    {
        var potentialExits = FindObjectsOfType<PotentialExit>();
        if (potentialExits.Length < 1) return;
        potentialExits[Random.Range(0, potentialExits.Length)].SetAsExit(true);
        SetGameState(Beginning);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            if (gameState == Playing)
                SetGameState(Pause);
            else if (gameState == Pause)
                SetGameState(Playing);
        }
    }

    public void SetGameState(GameState actual)
    {
        var oldGameState = gameState;
        gameState = actual;
        
        Time.timeScale = gameState == Playing ? 1 : 0;

        onGameStateChanged.Invoke(oldGameState, gameState);
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