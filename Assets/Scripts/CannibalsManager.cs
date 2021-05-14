using UnityEngine;
using UnityEngine.AI;

public class CannibalsManager : Singleton<CannibalsManager>
{
    [HideInInspector] public SoundManager.EventAmbiance onAmbianceChanged;
    
    [SerializeField] private CannibalsState state = CannibalsState.Following;

    private EnemyFollow[] _cannibals;
    private Transform _player;

    private void Start()
    {
        _cannibals = FindObjectsOfType<EnemyFollow>();
        _player = PlayerController.Instance.GetComponent<Transform>();
        SetState(CannibalsState.Following);
        InvokeRepeating(nameof(FollowTrace), 0, 5);
    }

    private void FollowTrace()
    {
        if (state == CannibalsState.Following || !AreaStrategy.IsInitialized || GameManager.Instance.gameState != GameManager.GameState.Playing)
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

    private void Update()
    {
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