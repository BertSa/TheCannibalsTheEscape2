using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static GameManager;
using Random = UnityEngine.Random;

public class EnemyFollow : MonoBehaviour
{
    public const int DistanceToAttack = 2;
    private const int Acceleration = 1;
    private readonly int _attack = Animator.StringToHash("Attack");

    [SerializeField] private int speed = 5;
    [Header("Clips")] [SerializeField] private AudioClip[] attack;
    [SerializeField] private AudioClip[] follow;
    [SerializeField] private AudioClip[] searching;

    private NavMeshAgent _agent;
    private Transform _player;
    private Animator _animator;

    #region Audio

    private readonly IEnumerator[] fader = new IEnumerator[2];
    private int _activePlayer;

    private const int VolumeChangesPerSecond = 15;
    private const float FadeDuration = 1.0f;
    private const float Volume = 0.5f;
    private AudioSource[] _clipPlaying;

    #endregion

    private void Awake()
    {
        _clipPlaying = new[]
        {
            gameObject.AddComponent<AudioSource>(),
            gameObject.AddComponent<AudioSource>()
        };

        foreach (var s in _clipPlaying)
        {
            s.loop = true;
            s.playOnAwake = false;
            s.volume = 0.0f;
            s.spatialBlend = 1;
        }
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.SetBool(_attack, false);
        _agent = GetComponent<NavMeshAgent>();
        _player = PlayerController.Instance.GetComponent<Transform>();

        _agent.acceleration = Acceleration;
        _agent.speed = speed;
        _agent.autoRepath = true;
        Play(follow);
    }

    private void Update()
    {
        var playerPosition = _player.position;

        _agent.SetDestination(playerPosition);
        
        var isNearPlayer = IsNearPlayer(2, DistanceToAttack);
        
        Play(isNearPlayer ? attack : follow);
        
        _animator.SetBool(_attack, isNearPlayer);
        
        transform.rotation = Quaternion.LookRotation(playerPosition - transform.position);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && GameManager.IsInitialized)
            GameManager.Instance.SetGameState(GameState.LostCannibals);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("areaPlayer"))
        {
        }
    }

    private bool IsPlayerInArea()
    {
        const int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];
        var numColliders = Physics.OverlapSphereNonAlloc(transform.position, 50, hitColliders);
        return false;
    }
    
    public void MoveToward( Vector3 targetPoint)
    {
        Quaternion rotation = Quaternion.LookRotation(targetPoint - transform.position);
        rotation.x = 0f;
        rotation.z = 0f;
        
        Transform transform1 = transform;
        
        transform1.rotation = Quaternion.Slerp(transform1.rotation, rotation, Time.deltaTime * _agent.angularSpeed);
 
        Vector3 movementVelocity = transform1.forward * speed;
 
        movementVelocity.y = -.08f;
    }


    public bool IsNearPlayer(float rateDistance, int distance)
    {
        if (rateDistance <= 0)
            throw new ArgumentOutOfRangeException(nameof(rateDistance) + " cannot be lower or equals than 0");

        if (distance <= 0)
            throw new ArgumentOutOfRangeException(nameof(distance) + " cannot be lower or equals than 0");

        return Vector3.Distance(transform.position, _player.position) <= distance * rateDistance;
    }


    private void Play(IReadOnlyList<AudioClip> clips)
    {
        var clip = clips[Random.Range(0, clips.Count)];
        if ((clips == follow && _clipPlaying[_activePlayer].isPlaying) || clip == _clipPlaying[_activePlayer].clip)
            return;

        foreach (var i in fader)
            if (i != null)
                StopCoroutine(i);

        if (_clipPlaying[_activePlayer].volume > 0)
        {
            fader[0] = FadeAudioSource(_clipPlaying[_activePlayer], FadeDuration, 0.0f, () => { fader[0] = null; });
            StartCoroutine(fader[0]);
        }

        var nextPlayer = (_activePlayer + 1) % _clipPlaying.Length;
        _clipPlaying[nextPlayer].clip = clip;
        _clipPlaying[nextPlayer].Play();
        fader[1] = FadeAudioSource(_clipPlaying[nextPlayer], FadeDuration, Volume, () => { fader[1] = null; });
        StartCoroutine(fader[1]);

        _activePlayer = nextPlayer;
    }

    private static IEnumerator FadeAudioSource(AudioSource player, float duration, float targetVolume, Action finishedCallback)
    {
        var steps = (int) (VolumeChangesPerSecond * duration);
        var stepTime = duration / steps;
        var stepSize = (targetVolume - player.volume) / steps;

        for (var i = 1; i < steps; i++)
        {
            player.volume += stepSize;
            yield return new WaitForSeconds(stepTime);
        }
        player.volume = targetVolume;
        finishedCallback?.Invoke();
    }
}