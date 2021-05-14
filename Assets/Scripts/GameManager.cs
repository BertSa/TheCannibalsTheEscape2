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
        if (!Input.GetKey(KeyCode.P)) return;
        if (gameState == Playing)
            SetGameState(Pause);
        else if (gameState == Pause)
            SetGameState(Playing);
    }

    public void SetGameState(GameState actual)
    {
        var oldGameState = gameState;
        gameState = actual;
        
        if (oldGameState == Beginning && actual == Playing)
            CannibalsManager.Instance.SetState(CannibalsManager.CannibalsState.Following);
        
        Time.timeScale = gameState == Playing ? 1 : 0;

        onGameStateChanged.Invoke(oldGameState, gameState);
    }

    public enum GameState
    {
        Playing,
        LostCannibals,
        LostTorch,
        Won,
        Pause,
        Beginning
    }
}