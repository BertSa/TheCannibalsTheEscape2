using System.Collections.Generic;
using UnityEngine;
using static System.Single;
using Random = UnityEngine.Random;

public class SoundManager : Singleton<SoundManager>
{
    private AudioSource[] findObjectsOfType;
    private AudioSource _audioSource;
    [Header("Clips")] [SerializeField] private AudioClip ending;
    [SerializeField] private List<AudioClip> ambiances;

    private Transform _player;

    protected override void Awake()
    {
        base.Awake();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (PlayerController.IsInitialized) _player = PlayerController.Instance.GetComponent<Transform>();
        var pos = RandomCircle(_player.position, 6.0f);
        AudioSource.PlayClipAtPoint(ambiances[Random.Range(0, ambiances.Count)], pos,MaxValue);
    }


    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable Unity.PerformanceAnalysis
    public void PauseGame()
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
        pos.x = center.x + minDistance +(radius * Mathf.Sin(ang * Mathf.Deg2Rad));
        pos.y = center.y + Random.Range(0, 7);
        pos.z = center.z + minDistance + (radius * Mathf.Cos(ang * Mathf.Deg2Rad));
        return pos;
    }
}
