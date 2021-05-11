using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static System.Single;
using static CannibalsManager;
using static CannibalsManager.CannibalsState;
using static GameManager.GameState;
using Random = UnityEngine.Random;

public class SoundManager : Singleton<SoundManager>
{
    private AudioSource[] findObjectsOfType;
    private AudioSource _audioSource;
    [Header("Clips")] [SerializeField] private AudioClip ending;
    [SerializeField] private List<AudioClip> ambiances;
    [SerializeField] private List<AudioClip> alone;

    private Transform _player;
    [SerializeField] private AudioClip running;

    protected override void Awake()
    {
        base.Awake();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (PlayerController.IsInitialized) _player = PlayerController.Instance.GetComponent<Transform>();
        var pos = RandomCircle(_player.position, 6.0f);
        AudioSource.PlayClipAtPoint(ambiances[Random.Range(0, ambiances.Count)], pos, MaxValue);
        Singleton<CannibalsManager>.Instance.onAmbianceChanged.AddListener(HandleAmbianceChanged);
        GameManager.Instance.onGameStateChanged.AddListener(HandleGameStateChanged);
    }

    private void HandleGameStateChanged(GameManager.GameState previous, GameManager.GameState actual)
    {
        switch (actual)
        {
            case Playing:
                break;
            case Won:
            case LostCannibals:
            case LostTorch:
                EndGame();
                break;
            case Pause:
                PauseGame();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actual), actual, null);
        }
    }

    private void HandleAmbianceChanged(CannibalsState previous, CannibalsState actual)
    {
        if (previous == Searching && actual == Following)
        {
            _audioSource.clip = running;
            _audioSource.Play();
        }
        else if (actual == Searching)
        {
        }
        else if (actual == Attacking)
        {
            
        }
        else if (actual== Following)
        {
            
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(actual), actual, null);
        }
    }


    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable Unity.PerformanceAnalysis
    private void PauseGame()
    {
        findObjectsOfType = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in findObjectsOfType)
        {
            audioSource.Pause();
        }
    }

    public void EndGame()
    {
        PauseGame();
        _audioSource.clip = ending;
        _audioSource.Play();
    }

    private static Vector3 RandomCircle(Vector3 center, float radius)
    {
        const float minDistance = 3f;
        var ang = Random.value * 360;
        Vector3 pos;
        pos.x = center.x + minDistance + (radius * Mathf.Sin(ang * Mathf.Deg2Rad));
        pos.y = center.y + Random.Range(0, 7);
        pos.z = center.z + minDistance + (radius * Mathf.Cos(ang * Mathf.Deg2Rad));
        return pos;
    }

    [Serializable]
    public class EventAmbiance : UnityEvent<CannibalsState, CannibalsState>
    {
    }
}