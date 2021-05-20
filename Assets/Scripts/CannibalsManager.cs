using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class CannibalsManager : Singleton<CannibalsManager>
{
    public Events.EventAmbiance onAmbianceChanged = new Events.EventAmbiance();
    
    [SerializeField] private CannibalsState state = CannibalsState.Following;

    private EnemyFollow[] _cannibals;
    private Transform _player;

    private void Start()
    {
        _cannibals = FindObjectsOfType<EnemyFollow>();
        _player = PlayerController.Instance.GetComponent<Transform>();
        GameManager.Instance.onGameStateChanged.AddListener(HandleGameStateChanged);
    }

    private void HandleGameStateChanged(GameManager.GameState oldGameState, GameManager.GameState actual)
    {
        switch (actual)
        {
            case GameManager.GameState.Playing:
                if (oldGameState == GameManager.GameState.Beginning)
                    SetState(CannibalsState.Following);
                InvokeRepeating(nameof(FollowTrace), 0, 5);
                break;
            case GameManager.GameState.Beginning:
            case GameManager.GameState.LostCannibals:
            case GameManager.GameState.LostTorch:
            case GameManager.GameState.Won:
            case GameManager.GameState.Pause:
                CancelInvoke(nameof(FollowTrace));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actual), actual, null);
        }
    }

    private void FollowTrace()
    {
        if (state == CannibalsState.Following || 
            GameManager.Instance.gameState != GameManager.GameState.Playing)
            return;

        foreach (var cannibal in _cannibals)
            cannibal.SetDestination(GetRandomPoint(_player.position, AreaStrategy.Instance.GetRadius()));
    }
    
    public static Vector3 GetRandomPoint(Vector3 center, float maxDistance)
    {
        var randomPos = Random.insideUnitSphere * maxDistance + center;

        NavMesh.SamplePosition(randomPos, out var hit, maxDistance, NavMesh.AllAreas);

        return hit.position;
    }

    public void SetState(CannibalsState actual)
    {
        onAmbianceChanged.Invoke(state, actual);
        state = actual;
    }

    public enum CannibalsState
    {
        Following,
        Searching
    }

    public CannibalsState GetState()
    {
        return state;
    }
}