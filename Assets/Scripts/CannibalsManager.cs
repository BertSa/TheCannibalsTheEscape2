using System;
using Enums;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class CannibalsManager : Singleton<CannibalsManager>
{
    public readonly Events.EventAmbiance OnAmbianceChanged = new();

    public CannibalsState State { get; private set; } = CannibalsState.Following;

    private EnemyFollow[] Cannibals { get; set; }
    private Transform Player { get; set; }

    private void Start()
    {
        Cannibals = FindObjectsOfType<EnemyFollow>();
        Player = PlayerController.Instance.GetComponent<Transform>();
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
    }

    private void HandleGameStateChanged(GameState oldGameState, GameState actual)
    {
        if (actual != GameState.Playing)
        {
            CancelInvoke(nameof(FollowTrace));
        }

        if (oldGameState == GameState.Beginning)
        {
            UpdateState(CannibalsState.Following);
        }

        InvokeRepeating(nameof(FollowTrace), 0, 5);
    }

    private void FollowTrace()
    {
        if (State == CannibalsState.Following)
        {
            return;
        }

        if (GameManager.Instance.State != GameState.Playing)
        {
            return;
        }

        foreach (var cannibal in Cannibals)
        {
            var randomPoint = GetRandomPoint(Player.position, AreaStrategy.Instance.GetRadius());
            cannibal.SetDestination(randomPoint);
        }
    }

    public static Vector3 GetRandomPoint(Vector3 center, float maxDistance)
    {
        var randomPos = Random.insideUnitSphere * maxDistance + center;

        NavMesh.SamplePosition(randomPos, out var hit, maxDistance, NavMesh.AllAreas);

        return hit.position;
    }

    public void UpdateState(CannibalsState actual)
    {
        OnAmbianceChanged.Invoke(State, actual);
        State = actual;
    }
}