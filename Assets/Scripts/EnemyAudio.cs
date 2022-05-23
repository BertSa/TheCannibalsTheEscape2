using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAudio : MonoBehaviour
{
    private const float Volume = 0.5f;
    private const float FadeDuration = 1f;
    private const int VolumeChangesPerSecond = 15;

    private readonly IEnumerator[] _fader = new IEnumerator[2];

    [Header("Clips")] [SerializeField] private AudioClip[] attackSounds;
    [SerializeField] private AudioClip[] followingSounds;
    [SerializeField] private AudioClip[] searchingSounds;

    private AudioSource[] ClipsPlaying { get; set; }
    private int ActivePlayerIndex { get; set; }
    private EnemyFollow Enemy { get; set; }

    private void Awake()
    {
        Enemy = GetComponent<EnemyFollow>();

        ClipsPlaying = new[] { gameObject.AddComponent<AudioSource>(), gameObject.AddComponent<AudioSource>(), };

        foreach (var source in ClipsPlaying)
        {
            source.loop = true;
            source.playOnAwake = false;
            source.volume = 0.0f;
            source.spatialBlend = 1;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing || CannibalsManager.Instance.State != CannibalsState.Following)
        {
            return;
        }

        var audioClips = GetClips();

        Play(audioClips);
    }

    private AudioClip[] GetClips()
    {
        if (CannibalsManager.Instance.State == CannibalsState.Searching)
        {
            return searchingSounds;
        }

        return Enemy.IsNearPlayer ? attackSounds : followingSounds;
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