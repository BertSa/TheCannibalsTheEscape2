using System;
using Enums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static CannibalsManager;
using static Enums.CannibalsState;
using static Enums.GameState;
using Random = UnityEngine.Random;

public class SoundManager : Singleton<SoundManager>
{
    [Header("Clips")] [SerializeField] private AudioClip lostCannibals;
    [SerializeField] private AudioClip lostTorch;
    [SerializeField] private AudioClip win;
    [SerializeField] private AudioClip pauseClip;
    [SerializeField] private AudioClip[] ambianceSearchingClip;
    [SerializeField] private AudioClip[] ambianceFollowingClip;
    private AudioSource _audioSource;
    private AudioSource _pauseAudioSource;
    private AudioSource[] _findObjectsOfType;

    private Transform _playerTransform;

    protected override void Awake()
    {
        base.Awake();
        _audioSource = gameObject.AddComponent<AudioSource>();
        _pauseAudioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.loop = false;
        _pauseAudioSource.loop = true;
        _pauseAudioSource.clip = pauseClip;
    }

    private void Start()
    {
        _playerTransform = PlayerController.Instance.GetComponent<Transform>();

        CannibalsManager.Instance.OnAmbianceChanged.AddListener(HandleAmbianceChanged);
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
    }

    private void RandomSound()
    {
        if (CannibalsManager.Instance.State == Following)
        {
            return;
        }

        float timeRandomSound;
        if (_audioSource.isPlaying)
        {
            var size = _audioSource.clip.length;
            timeRandomSound = Random.Range(size, size + 30);
            Invoke(nameof(RandomSound), timeRandomSound);
            return;
        }

        var selectedClip = Random.Range(0, ambianceSearchingClip.Length);
        var audioClip = ambianceSearchingClip[selectedClip];
        var randomPos = GetRandomPoint(_playerTransform.position, 3.0f);

        AudioSource.PlayClipAtPoint(audioClip, randomPos, 1);

        timeRandomSound = Random.Range(60, 250);
        Invoke(nameof(RandomSound), timeRandomSound);
    }

    private void HandleGameStateChanged(GameState previous, GameState actual)
    {
        switch (actual)
        {
            case Beginning:
                PauseGameSounds();
                break;
            case Pause:
                PauseGameSounds();
                _pauseAudioSource.Play();
                break;
            case Playing:
                Play();
                break;
            case Won:
                EndGameSound(win);
                break;
            case LostCannibals:
                EndGameSound(lostCannibals);
                break;
            case LostTorch:
                EndGameSound(lostTorch);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actual), actual, null);
        }
    }

    private void Play()
    {
        _findObjectsOfType = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in _findObjectsOfType)
        {
            audioSource.Play();
        }

        _pauseAudioSource.Stop();
    }

    private void HandleAmbianceChanged(CannibalsState previous, CannibalsState actual)
    {
        switch (actual)
        {
            case Searching:
                var timeRandomSound = Random.Range(10, 12);
                Invoke(nameof(RandomSound), timeRandomSound);
                break;
            case Following when _audioSource.isPlaying:
                return;
            case Following:
                var selected = Random.Range(0, ambianceFollowingClip.Length);
                _audioSource.clip = ambianceFollowingClip[selected];
                _audioSource.Play();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actual), actual, null);
        }
    }

    private void PauseGameSounds()
    {
        _findObjectsOfType = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in _findObjectsOfType)
        {
            audioSource.Pause();
        }
    }

    private void EndGameSound(AudioClip clip)
    {
        PauseGameSounds();
        _audioSource.clip = clip;
        _audioSource.Play();
    }
}