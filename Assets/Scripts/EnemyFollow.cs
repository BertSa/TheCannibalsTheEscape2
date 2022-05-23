using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyFollow : MonoBehaviour
{
    private const int DistanceToAttack = 4;
    private readonly int _attack = Animator.StringToHash("Attack");

    [SerializeField] private int speed = 3;

    private NavMeshAgent Agent { get; set; }
    private Transform Player { get; set; }
    private Animator Anim { get; set; }
    private Rigidbody Rb { get; set; }

    [Header("Clips")] [SerializeField] private AudioClip[] attackSounds;
    [SerializeField] private AudioClip[] followingSounds;
    [SerializeField] private AudioClip[] searchingSounds;
    private AudioSource[] ClipsPlaying { get; set; }
    private int ActivePlayerIndex { get; set; }

    private readonly IEnumerator[] _fader = new IEnumerator[2];

    private const int VolumeChangesPerSecond = 15;
    private const float FadeDuration = 1.0f;
    private const float Volume = 0.5f;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        Anim = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();

        ClipsPlaying = new[] { gameObject.AddComponent<AudioSource>(), gameObject.AddComponent<AudioSource>(), };

        foreach (var source in ClipsPlaying)
        {
            source.loop = true;
            source.playOnAwake = false;
            source.volume = 0.0f;
            source.spatialBlend = 1;
        }
    }

    private void Start()
    {
        Player = PlayerController.Instance.GetComponent<Transform>();
        Anim.SetBool(_attack, false);

        Agent.speed = speed;
        Agent.autoRepath = true;
        Agent.autoBraking = true;
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing || CannibalsManager.Instance.State != CannibalsState.Following)
        {
            return;
        }

        var isNearPlayer = IsNearPlayer(DistanceToAttack);

        AudioClip[] sounds;
        if (CannibalsManager.Instance.State == CannibalsState.Searching)
        {
            sounds = searchingSounds;
        }
        else
        {
            sounds = isNearPlayer ? attackSounds : followingSounds;
        }

        Play(sounds);

        SetDestination(Player.position);

        RotateToward(Agent.destination);

        Anim.SetBool(_attack, isNearPlayer);
    }

    public void SetDestination(Vector3 destination)
    {
        Agent.SetDestination(destination);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.SetGameState(GameState.LostCannibals);
        }
    }

    private void RotateToward(Vector3 targetPoint)
    {
        var rotation = Quaternion.LookRotation(targetPoint - transform.position);
        rotation.x = 0f;
        rotation.z = 0f;

        var t = transform;

        t.rotation = Quaternion.Slerp(t.rotation, rotation, Time.deltaTime * Agent.angularSpeed);

        var movementVelocity = t.forward * speed;

        movementVelocity.y = -.8f;

        Rb.velocity = movementVelocity;
    }

    private bool IsNearPlayer(float distance)
    {
        if (distance <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(distance) + " cannot be lower or equals than 0");
        }

        return Vector3.Distance(transform.position, Player.position) <= distance;
    }

    private void Play(IReadOnlyList<AudioClip> clips)
    {
        if (clips == followingSounds && ClipsPlaying[ActivePlayerIndex].isPlaying)
        {
            return;
        }

        var selectedSound = Random.Range(0, clips.Count);
        var clip = clips[selectedSound];

        if (clip == ClipsPlaying[ActivePlayerIndex].clip)
        {
            return;
        }

        foreach (var enumerator in _fader)
        {
            if (enumerator != null)
            {
                StopCoroutine(enumerator);
            }
        }

        if (ClipsPlaying[ActivePlayerIndex].volume > 0)
        {
            _fader[0] = FadeAudioSource(ClipsPlaying[ActivePlayerIndex], FadeDuration, 0.0f, () =>
            {
                _fader[0] = null;
            });
            StartCoroutine(_fader[0]);
        }

        var nextPlayer = (ActivePlayerIndex + 1) % ClipsPlaying.Length;
        ClipsPlaying[nextPlayer].clip = clip;
        ClipsPlaying[nextPlayer].Play();
        _fader[1] = FadeAudioSource(ClipsPlaying[nextPlayer], FadeDuration, Volume, () =>
        {
            _fader[1] = null;
        });
        StartCoroutine(_fader[1]);

        ActivePlayerIndex = nextPlayer;
    }


    private static IEnumerator FadeAudioSource(AudioSource player, float duration, float targetVolume, Action finishedCallback)
    {
        var steps = (int)(VolumeChangesPerSecond * duration);
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