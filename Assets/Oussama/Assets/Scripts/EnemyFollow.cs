using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;
using static GameManager;
using Random = UnityEngine.Random;

public class EnemyFollow : MonoBehaviour
{
    private const int DistanceToAttack = 4;
    private const int Acceleration = 1;
    private readonly int _attack = Animator.StringToHash("Attack");
    private bool _alreadyNotified = false;

    [SerializeField] private int speed = 7;

    private NavMeshAgent _agent;
    private Transform _player;
    private Animator _animator;
    private Rigidbody _rb;

    #region Audio

    [Header("Clips")] [SerializeField] private AudioClip[] attack;
    [SerializeField] private AudioClip[] follow;
    [SerializeField] private AudioClip[] searching;

    private readonly IEnumerator[] _fader = new IEnumerator[2];

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

        foreach (var source in _clipPlaying)
        {
            source.loop = true;
            source.playOnAwake = false;
            source.volume = 0.0f;
            source.spatialBlend = 1;
        }
    }

    private void Start()
    {
        if(CannibalsManager.IsInitialized)
            CannibalsManager.Instance.destinationChangeEvent.AddListener(HandleDestinationChange);
        
        _rb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _player = PlayerController.Instance.GetComponent<Transform>();
        _animator.SetBool(_attack, false);

        _agent.acceleration = Acceleration;
        _agent.speed = speed;
        _agent.autoRepath = true;
        _agent.destination = _player.position;
        _agent.autoBraking = false;
        Play(follow);
    }

    private void Update()
    {
        var playerPosition = _player.position;

        if(!_alreadyNotified)
            SetDestination(playerPosition);
        else
            SetDestination(CannibalsManager.Instance.waypoints[Random.Range(0, CannibalsManager.Instance.waypoints.Count)]);

        var isNearPlayer = IsNearPlayer(DistanceToAttack);

        Play(isNearPlayer ? attack : follow);

        MoveToward(_agent.destination);
        
        _animator.SetBool(_attack, isNearPlayer);
    }

    public void SetDestination(Vector3 destination)
    {
        _agent.SetDestination(destination);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && GameManager.IsInitialized)
            GameManager.Instance.SetGameState(GameState.LostCannibals);
    }

    private void MoveToward( Vector3 targetPoint)
    {
        var rotation = Quaternion.LookRotation(targetPoint - transform.position);
        rotation.x = 0f;
        rotation.z = 0f;
        
        var t = transform;
        
        t.rotation = Quaternion.Slerp(t.rotation, rotation, Time.deltaTime * _agent.angularSpeed);
    
        var movementVelocity = t.forward * speed;
    
        movementVelocity.y = -.1f;
        
        _rb.velocity = movementVelocity;
    }

    private bool IsNearPlayer(float distance)
    {
        if (distance <= 0)
            throw new ArgumentOutOfRangeException(nameof(distance) + " cannot be lower or equals than 0");

        return Vector3.Distance(transform.position, _player.position) <= distance;
    }

    private void Play(IReadOnlyList<AudioClip> clips)
    {
        var clip = clips[Random.Range(0, clips.Count)];
        if (clips == follow && _clipPlaying[_activePlayer].isPlaying || clip == _clipPlaying[_activePlayer].clip)
            return;

        foreach (var enumerator in _fader)
            if (enumerator != null)
                StopCoroutine(enumerator);

        if (_clipPlaying[_activePlayer].volume > 0)
        {
            _fader[0] = FadeAudioSource(_clipPlaying[_activePlayer], FadeDuration, 0.0f, () => { _fader[0] = null; });
            StartCoroutine(_fader[0]);
        }

        var nextPlayer = (_activePlayer + 1) % _clipPlaying.Length;
        _clipPlaying[nextPlayer].clip = clip;
        _clipPlaying[nextPlayer].Play();
        _fader[1] = FadeAudioSource(_clipPlaying[nextPlayer], FadeDuration, Volume, () => { _fader[1] = null; });
        StartCoroutine(_fader[1]);

        _activePlayer = nextPlayer;
    }
    
    private void HandleDestinationChange(Vector3 destination)
    {
        if(_agent.destination != destination && _alreadyNotified)
            _agent.SetDestination(destination);
        _alreadyNotified = true;
    }

    private static IEnumerator FadeAudioSource(AudioSource player, float duration, float targetVolume,
        Action finishedCallback)
    {
        var steps = (int) (VolumeChangesPerSecond * duration);
        var stepTime = duration / steps;
        var stepSize = (targetVolume - player.volume) / steps;

        for (var i = 1; i < steps; ++i)
        {
            player.volume += stepSize;
            yield return new WaitForSeconds(stepTime);
        }

        player.volume = targetVolume;
        finishedCallback?.Invoke();
    }
}