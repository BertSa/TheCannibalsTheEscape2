using System;
using UnityEngine;
using UnityEngine.Events;
using static CannibalsManager;
using static CannibalsManager.CannibalsState;
using static GameManager.GameState;
using Random = UnityEngine.Random;

public class SoundManager : Singleton<SoundManager>
{
    [Header("Clips")] [SerializeField] private AudioClip badEnding;
    private AudioSource _audioSource;

    private AudioSource[] _findObjectsOfType;
    private Transform _player;

    protected override void Awake()
    {
        base.Awake();
        _audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        // if (PlayerController.IsInitialized) _player = PlayerController.Instance.GetComponent<Transform>();
        // var pos = RandomCircle(_player.position, 6.0f);
        // AudioSource.PlayClipAtPoint(following[Random.Range(0, following.Count)], pos, MaxValue);
        CannibalsManager.Instance.onAmbianceChanged.AddListener(HandleAmbianceChanged);
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
            // _audioSource.clip = ;
            // _audioSource.Play();
        }
        else if (actual == Searching)
        {
        }
        else if (actual == Following)
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
        _findObjectsOfType = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in _findObjectsOfType) audioSource.Pause();
    }

    public void EndGame()
    {
        PauseGame();
        _audioSource.clip = badEnding;
        _audioSource.Play();
    }

    private static Vector3 RandomCircle(Vector3 center, float radius)
    {
        const float minDistance = 3f;
        var ang = Random.value * 360;
        var pos = center;
        pos.x += minDistance + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y += Random.Range(0, 7);
        pos.z += minDistance + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }

    [Serializable]
    public class EventAmbiance : UnityEvent<CannibalsState, CannibalsState>
    {
    }
}