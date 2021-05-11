using System;
using UnityEngine;
using static CannibalsManager.CannibalsState;
using static SoundManager;

public class CannibalsManager : Singleton<CannibalsManager>
{

    private EnemyFollow[] cannibals;
    [SerializeField] private CannibalsState state = Following;
    [HideInInspector] public EventAmbiance onAmbianceChanged;
    
    private Transform player;
    
    protected override void Awake()
    {
        base.Awake();
    }
    
    private void Start()
    {
        cannibals = FindObjectsOfType<EnemyFollow>();
        player = PlayerController.Instance.GetComponent<Transform>();
        SetState(Following);
    }

    private void Update()
    {
        foreach (var c in cannibals)
        {
            var cannibalPosition = c.transform;
            var dirFromAtoB = (player.transform.position - cannibalPosition.position).normalized;
            var dotProd = Vector3.Dot(dirFromAtoB, cannibalPosition.forward);

            var lookingAtPlayer = dotProd >= 0 && dotProd <= 1;
            
            if(lookingAtPlayer)
            {
                SetState(Following);
                return;
            }
            if (c.IsNearPlayer(1, EnemyFollow.DistanceToAttack))
            {
                SetState(Attacking);
                return;
            }
            SetState(Searching);
            return;
        }
    }


    public void SetState(CannibalsState actual)
    {
        var previous = state;
        state = actual;
        
        onAmbianceChanged.Invoke(previous, actual);
    }


    public enum CannibalsState
    {
        Following,
        Searching,
        Attacking
    }
}
