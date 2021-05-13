using UnityEngine;
using UnityEngine.AI;
using static CannibalsManager.CannibalsState;
using static SoundManager;

public class CannibalsManager : Singleton<CannibalsManager>
{
    private EnemyFollow[] _cannibals;
    [SerializeField] private CannibalsState state = Following;
    [HideInInspector] public EventAmbiance onAmbianceChanged;

    private bool _alreadySetDestination;
    private Transform _player;

    private void Start()
    {
        _cannibals = FindObjectsOfType<EnemyFollow>();
        _player = PlayerController.Instance.GetComponent<Transform>();
        SetState(Following);
        InvokeRepeating(nameof(FollowTrace), 0, 5);
    }

    private void FollowTrace()
    {
        if (state == Following || !AreaStrategy.IsInitialized)
            return;
        
        foreach (var cannibal in _cannibals)
            cannibal.SetDestination(GetRandomPoint(_player.position,AreaStrategy.Instance.GetRadius()));
    }

    private static Vector3 GetRandomPoint(Vector3 center, float maxDistance) {
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
        if (state != Following) return;
        foreach (var cannibal in _cannibals)
        {
            cannibal.SetDestination(_player.position);
        }
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