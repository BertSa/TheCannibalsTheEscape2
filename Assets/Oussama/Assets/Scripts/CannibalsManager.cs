using System;
using UnityEngine;
using static CannibalsManager.CannibalsState;
using static SoundManager;

public class CannibalsManager : Singleton<CannibalsManager>
{

    private EnemyFollow[] canibals;
    [SerializeField] private CannibalsState state = Following;
    [HideInInspector] public EventAmbiance onAmbianceChanged;


    [SerializeField] private AudioClip[] attack;
    [SerializeField] private AudioClip[] follow;
    [SerializeField] private AudioClip[] searching;

    private Transform player;
    private CannibalsState currentCannibalsState;
    
    protected override void Awake()
    {
        base.Awake();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        canibals = FindObjectsOfType<EnemyFollow>();
        player = PlayerController.Instance.GetComponent<Transform>();
        currentCannibalsState = Following;
        SetState(Following);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var c in canibals)
        {
            var cannibalPosition = c.transform;
            Vector3 dirFromAtoB = (player.transform.position - cannibalPosition.position).normalized;
            float dotProd = Vector3.Dot(dirFromAtoB, cannibalPosition.forward);

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
        switch (actual)
        {
            case Following:
                foreach (var canibal in canibals)
                {
                    // var audioSource = canibal.gameObject.AddComponent<AudioSource>();
                    // audioSource.clip = follow[0];
                    // audioSource.loop = true;
                    // audioSource.Play();
                }

                break;
            case Searching:
                break;
            case Attacking:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actual), actual, null);
        }
        currentCannibalsState = actual;
    }


    public enum CannibalsState
    {
        Following,
        Searching,
        Attacking
    }
}
