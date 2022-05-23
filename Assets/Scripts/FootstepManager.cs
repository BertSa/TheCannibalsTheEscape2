using System;
using UnityEngine;
using static Enums.GameState;

public class FootstepManager : MonoBehaviour
{
    [Header("Audio")] private AudioSource _audioSource;
    [SerializeField] private AudioClip running;
    [SerializeField] private AudioClip walking;

    private PlayerController _player;

    private void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.loop = true;
    }

    private void Start()
    {
        _player = PlayerController.Instance;
    }

    private void Update()
    {
        if (GameManager.Instance.State != Playing)
        {
            return;
        }

        if (!_player.IsSprinting && !_player.IsWalking && !_audioSource.isPlaying)
        {
            return;
        }

        if (_player.IsSprinting)
        {
            if (_audioSource.isPlaying && _audioSource.clip == running)
            {
                return;
            }

            _audioSource.clip = running;
            _audioSource.Play();
        }
        else if (_player.IsWalking)
        {
            if (_audioSource.isPlaying && _audioSource.clip == walking)
            {
                return;
            }

            _audioSource.clip = walking;
            _audioSource.Play();
        }
        else
        {
            _audioSource.Stop();
        }
    }
}