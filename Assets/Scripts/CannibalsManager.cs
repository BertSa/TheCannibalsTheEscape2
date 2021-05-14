using UnityEngine;
using UnityEngine.AI;

public class CannibalsManager : Singleton<CannibalsManager>
{
    private EnemyFollow[] _cannibals;
    [SerializeField] private CannibalsState state = CannibalsState.Following;
    [HideInInspector] public SoundManager.EventAmbiance onAmbianceChanged;

    private bool _alreadySetDestination;
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

    
    /// <summary>
    /// Gets a random point within maxdistance radius of a position in NavMeshSurface
    /// </summary>
    /// <param name="center">center of position</param>
    /// <param name="maxDistance">max distance for a random position</param>
    /// <returns>a random point within the NavMeshSurface </returns>
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
        if (GameManager.Instance.gameState != GameManager.GameState.Playing) return;
        if (state != CannibalsState.Following) return;
        foreach (var cannibal in _cannibals)
            cannibal.SetDestination(_player.position);
    }

    public enum CannibalsState
    {
        /// <summary>
        /// when cannibals are following the player
        /// </summary>
        Following,

        /// <summary>
        /// when cannibals are searching the player
        /// </summary>
        Searching,
    }

    public CannibalsState GetState()
    {
        return state;
    }
}