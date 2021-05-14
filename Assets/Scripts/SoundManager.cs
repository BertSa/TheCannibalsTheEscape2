using System;
using UnityEngine;
using UnityEngine.Events;
using static CannibalsManager;
using static CannibalsManager.CannibalsState;
using static GameManager.GameState;

public class SoundManager : Singleton<SoundManager>
{
    [Header("Clips")] 
    [SerializeField] private AudioClip lostCannibals; 
    [SerializeField] private AudioClip lostTorch;
    [SerializeField] private AudioClip win;
    
    [SerializeField] private AudioClip ambianceSearchingClip;
    [SerializeField] private AudioClip ambianceFollowingClip;
        
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
                _audioSource.clip = ambianceSearchingClip;
                _audioSource.Play();
                break;
            case Following:
                _audioSource.clip = ambianceFollowingClip;
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