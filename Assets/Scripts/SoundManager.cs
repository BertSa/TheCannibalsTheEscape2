using System;
using UnityEngine;
using UnityEngine.Events;
using static CannibalsManager;
using static CannibalsManager.CannibalsState;
using static GameManager.GameState;
using Random = UnityEngine.Random;

public class SoundManager : Singleton<SoundManager>
{
    [Header("Clips")] [SerializeField] private AudioClip lostCannibals;
    [SerializeField] private AudioClip lostTorch;
    [SerializeField] private AudioClip win;

    [SerializeField] private AudioClip[] ambianceSearchingClip;
    [SerializeField] private AudioClip[] ambianceFollowingClip;

    private AudioSource _audioSource;

    private AudioSource[] _findObjectsOfType;
    private Transform _player;

    protected override void Awake()
    {
        base.Awake();
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.loop = false;
    }

    private void Start()
    {
        CannibalsManager.Instance.onAmbianceChanged.AddListener(HandleAmbianceChanged);
        GameManager.Instance.onGameStateChanged.AddListener(HandleGameStateChanged);
    }

    private void RandomSound()
    {
        if (CannibalsManager.Instance.GetState() == Following) return;
        if (_audioSource.isPlaying)
        {
            var size = _audioSource.clip.length;
            Invoke(nameof(RandomSound), Random.Range(size, size + 30));
            return;
        }

        if (PlayerController.IsInitialized) _player = PlayerController.Instance.GetComponent<Transform>();
        var pos = GetRandomPoint(_player.position, 3.0f);
        AudioSource.PlayClipAtPoint(ambianceSearchingClip[Random.Range(0, ambianceSearchingClip.Length)], pos, 1);
        Invoke(nameof(RandomSound), Random.Range(60, 250));
    }

    private void HandleGameStateChanged(GameManager.GameState previous, GameManager.GameState actual)
    {
        switch (actual)
        {
            case Beginning:
            case Pause:
                PauseGame();
                break;
            case Playing:
                Play();
                break;
            case Won:
                EndGame(win);
                break;
            case LostCannibals:
                EndGame(lostCannibals);
                break;
            case LostTorch:
                EndGame(lostTorch);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actual), actual, null);
        }
    }

    private void Play()
    {
        _findObjectsOfType = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in _findObjectsOfType) audioSource.Play();
    }

    private void HandleAmbianceChanged(CannibalsState previous, CannibalsState actual)
    {
        switch (actual)
        {
            case Searching:
                Invoke(nameof(RandomSound), Random.Range(10, 12));
                break;
            case Following:
                if (_audioSource.isPlaying) return;
                _audioSource.clip = ambianceFollowingClip[Random.Range(0, ambianceFollowingClip.Length)];
                _audioSource.Play();
                break;
            default:
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

    private void EndGame(AudioClip clip)
    {
        PauseGame();
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    [Serializable]
    public class EventAmbiance : UnityEvent<CannibalsState, CannibalsState>
    {
    }
}