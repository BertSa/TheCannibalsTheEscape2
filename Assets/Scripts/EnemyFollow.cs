using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyFollow : MonoBehaviour
{
    #region Movement
    
    private const int DistanceToAttack = 4;
    private readonly int _attack = Animator.StringToHash("Attack");
    [SerializeField] private int speed = 3;
    private NavMeshAgent _agent;
    private Transform _player;
    private Animator _animator;
    private Rigidbody _rb;
    
    #endregion
    
    #region Audio

    [Header("Clips")] [SerializeField] private AudioClip[] attackSounds, followingSounds, searchingSounds;
    private AudioSource[] _clipsPlaying;

    private readonly IEnumerator[] _fader = new IEnumerator[2];

    private int _activePlayerIndex;

    private const int VolumeChangesPerSecond = 15;
    private const float FadeDuration = 1.0f, Volume = 0.5f;

    #endregion

    private void Awake()
    {
        _clipsPlaying = new[]
        {
            gameObject.AddComponent<AudioSource>(),
            gameObject.AddComponent<AudioSource>()
        };

        foreach (var source in _clipsPlaying)
        {
            source.loop = true;
            source.playOnAwake = false;
            source.volume = 0.0f;
            source.spatialBlend = 1;
        }
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        _animator = GetComponent<Animator>();
        _player = PlayerController.Instance.GetComponent<Transform>();
        _animator.SetBool(_attack, false);

        _agent = GetComponent<NavMeshAgent>();        
        _agent.speed = speed;
        _agent.autoRepath = true;
        _agent.autoBraking = true;
    }

    private void Update()
    {
        if (
            GameManager.Instance.gameState != GameManager.GameState.Playing || 
            CannibalsManager.Instance.GetState() != CannibalsManager.CannibalsState.Following)
            return;
        
        var isNearPlayer = IsNearPlayer(DistanceToAttack);

        Play(CannibalsManager.Instance.GetState() == CannibalsManager.CannibalsState.Searching ? searchingSounds : isNearPlayer ? attackSounds : followingSounds);
            
        SetDestination(_player.position);
        
        RotateToward(_agent.destination);

        _animator.SetBool(_attack, isNearPlayer);
    }

    public void SetDestination(Vector3 destination)
    {
        _agent.SetDestination(destination);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        GameManager.Instance.SetGameState(GameManager.GameState.LostCannibals);
    }

    private void RotateToward(Vector3 targetPoint)
    {
        var rotation = Quaternion.LookRotation(targetPoint - transform.position);
        rotation.x = 0f;
        rotation.z = 0f;

        var t = transform;

        t.rotation = Quaternion.Slerp(t.rotation, rotation, Time.deltaTime * _agent.angularSpeed);

        var movementVelocity = t.forward * speed;

        movementVelocity.y = -.8f;

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
        if (clips == followingSounds && _clipsPlaying[_activePlayerIndex].isPlaying || clip == _clipsPlaying[_activePlayerIndex].clip)
            return;

        foreach (var enumerator in _fader)
            if (enumerator != null)
                StopCoroutine(enumerator);

        if (_clipsPlaying[_activePlayerIndex].volume > 0)
        {
            _fader[0] = FadeAudioSource(_clipsPlaying[_activePlayerIndex], FadeDuration, 0.0f, () => { _fader[0] = null; });
            StartCoroutine(_fader[0]);
        }

        var nextPlayer = (_activePlayerIndex + 1) % _clipsPlaying.Length;
        _clipsPlaying[nextPlayer].clip = clip;
        _clipsPlaying[nextPlayer].Play();
        _fader[1] = FadeAudioSource(_clipsPlaying[nextPlayer], FadeDuration, Volume, () => { _fader[1] = null; });
        StartCoroutine(_fader[1]);

        _activePlayerIndex = nextPlayer;
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