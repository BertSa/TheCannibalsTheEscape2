using System;
using UnityEngine;
using UnityEngine.Events;
using static CannibalsManager.CannibalsState;
using static SoundManager;

public class CannibalsManager : Singleton<CannibalsManager>
{

    private EnemyFollow[] _cannibals;
    [SerializeField] private CannibalsState state = Following;
    [HideInInspector] public EventAmbiance onAmbianceChanged;

    [HideInInspector] public EventCannibalState destinationChangeEvent;
    
    private Transform _destination;
    
    private void Start()
    {
        _cannibals = FindObjectsOfType<EnemyFollow>();
        _destination = PlayerController.Instance.GetComponent<Transform>();
        SetState(Following);
    }

    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="actual"></param>
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
        /// <summary>
        /// when cannibals are following the player
        /// </summary>
        Following,
        
        /// <summary>
        /// when cannibals are searching the player
        /// </summary>
        Searching,
    }
    
    [Serializable]
    public class EventCannibalState : UnityEvent<Vector3>
    {
    }
}