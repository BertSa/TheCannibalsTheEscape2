using System;
using UnityEngine;
using static GameManager.GameState;

public class FootstepManager : MonoBehaviour
{
    #region Audio
    
    [Header("Audio")] private AudioSource _audioSource;
    [SerializeField] private AudioClip running;
    [SerializeField] private AudioClip walking;
    
    #endregion
    
    private PlayerController _player;

    private void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.loop = true;
    }

    private void Start()
    {
        if (PlayerController.IsInitialized) _player = PlayerController.Instance;
    }

    private void Update()
    {
        if (!_player.isSprinting && !_player.isWalking && !_audioSource.isPlaying ||
            GameManager.Instance.gameState != Playing) return;
        
        if (_player.isSprinting)
        {
            if (_audioSource.isPlaying && _audioSource.clip == running)
                return;
            _audioSource.clip = running;
            _audioSource.Play();
        }
        else if (_player.isWalking)
        {
            if (_audioSource.isPlaying && _audioSource.clip == walking)
                return;
            _audioSource.clip = walking;
            _audioSource.Play();
        }
        else
        {
            _audioSource.Stop();
        }
    }
}