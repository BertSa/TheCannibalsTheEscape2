using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using static CannibalsManager.CannibalsState;
using static SoundManager;

public class CannibalsManager : Singleton<CannibalsManager>
{

    private EnemyFollow[] _cannibals;
    [SerializeField] private CannibalsState state = Following;
    [HideInInspector] public EventAmbiance onAmbianceChanged;

    [HideInInspector] public EventCannibalState destinationChangeEvent;

    private bool _alreadySetDestination;
    private Transform _player;

    private Vector3 _currentDestination;
    public readonly List<Vector3> waypoints;

    private void Start()
    {
        _cannibals = FindObjectsOfType<EnemyFollow>();
        _player = PlayerController.Instance.GetComponent<Transform>();
        _currentDestination = _player.position;
        SetState(Following);
        InvokeRepeating(nameof(FollowTrace), 0, 5);
    }

    private void FollowTrace()
    {
        if (state == Following) return;

        if (_alreadySetDestination) return;
        foreach (var cannibal in _cannibals)
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 5;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 5, 1);
            Vector3 finalPosition = hit.position;

            cannibal.SetDestination(finalPosition);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actual"></param>
    public void SetState(CannibalsState actual)
    {
        onAmbianceChanged.Invoke(state, actual);
        switch (actual)
        {
            case Following:
                _currentDestination = _player.position;
                break;
            case Searching:
                break;
            default:
                throw new System.ArgumentOutOfRangeException();
        }
        destinationChangeEvent.Invoke(_currentDestination);
        state = actual;
    }

    private void Update()
    {
        var colliders = new Collider[100];
        List<Vector3> positions = null;
        if (Physics.OverlapSphereNonAlloc(_player.position, 20, colliders) > 0)
            positions = (from collider in colliders where collider.CompareTag("Cannibal") select collider.transform.position).ToList();

        if (positions == null)
            SetState(Searching);
        
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
    
    [System.Serializable]
    public class EventCannibalState : UnityEvent<Vector3>
    {
    }

    public CannibalsState GetState()
    {
        return state;
    }
}