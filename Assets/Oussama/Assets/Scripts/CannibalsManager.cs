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

    
    protected override void Awake()
    {
        base.Awake();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        canibals = FindObjectsOfType<EnemyFollow>();
        SetState(Following);
    }

    // Update is called once per frame
    void Update()
    {
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
                    var audioSource = canibal.gameObject.AddComponent<AudioSource>();
                    audioSource.clip = follow[0];
                    audioSource.loop = true;
                    audioSource.Play();
                }

                break;
            case Searching:
                break;
            case Attacking:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actual), actual, null);
        }
    }


    public enum CannibalsState
    {
        Following,
        Searching,
        Attacking
    }
}