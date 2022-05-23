using System;
using Enums;
using UnityEngine;
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

    private AudioSource Source { get; set; }
    private AudioSource PauseAudioSource { get; set; }
    private Transform PlayerTransform { get; set; }

    protected override void Awake()
    {
        base.Awake();
        Source = gameObject.AddComponent<AudioSource>();
        Source.loop = false;
        PauseAudioSource = gameObject.AddComponent<AudioSource>();
        PauseAudioSource.loop = true;
        PauseAudioSource.clip = pauseClip;
    }

    private void Start()
    {
        PlayerTransform = PlayerController.Instance.GetComponent<Transform>();

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
        if (Source.isPlaying)
        {
            var size = Source.clip.length;
            timeRandomSound = Random.Range(size, size + 30);
            Invoke(nameof(RandomSound), timeRandomSound);
            return;
        }

        var selectedClip = Random.Range(0, ambianceSearchingClip.Length);
        var audioClip = ambianceSearchingClip[selectedClip];
        var randomPos = GetRandomPoint(PlayerTransform.position, 3.0f);

        AudioSource.PlayClipAtPoint(audioClip, randomPos, 1);

        timeRandomSound = Random.Range(60, 250);
        Invoke(nameof(RandomSound), timeRandomSound);
    }

    private void HandleGameStateChanged(GameState previous, GameState actual)
    {
        switch (actual)
        {
            case Beginning:
                PauseAll();
                break;
            case Pause:
                PauseAll();
                PauseAudioSource.Play();
                break;
            case Playing:
                PlayAll();
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

    private void PlayAll()
    {
        var allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in allAudioSources)
        {
            audioSource.Play();
        }

        PauseAudioSource.Stop();
    }

    private void HandleAmbianceChanged(CannibalsState previous, CannibalsState actual)
    {
        switch (actual)
        {
            case Searching:
                var timeRandomSound = Random.Range(10, 12);
                Invoke(nameof(RandomSound), timeRandomSound);
                break;
            case Following when Source.isPlaying:
                return;
            case Following:
                var selected = Random.Range(0, ambianceFollowingClip.Length);
                Source.clip = ambianceFollowingClip[selected];
                Source.Play();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actual), actual, null);
        }
    }

    private void PauseAll()
    {
        var allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in allAudioSources)
        {
            audioSource.Pause();
        }
    }

    private void EndGame(AudioClip clip)
    {
        PauseAll();
        Source.clip = clip;
        Source.Play();
    }
}