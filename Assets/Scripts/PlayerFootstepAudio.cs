using System;
using UnityEngine;
using static Enums.GameState;

public class PlayerFootstepAudio : MonoBehaviour
{
    [Header("Audio")] [SerializeField] private AudioClip running;
    [SerializeField] private AudioClip walking;

    private AudioSource Source { get; set; }
    private PlayerController Player { get; set; }

    private void Awake()
    {
        Source = gameObject.AddComponent<AudioSource>();
        Source.loop = true;
    }

    private void Start()
    {
        Player = PlayerController.Instance;
    }

    private void Update()
    {
        if (GameManager.Instance.State != Playing)
        {
            return;
        }

        if (!Player.IsSprinting && !Player.IsWalking && !Source.isPlaying)
        {
            return;
        }

        if (Player.IsSprinting)
        {
            if (Source.isPlaying && Source.clip == running)
            {
                return;
            }

            Source.clip = running;
            Source.Play();
        }
        else if (Player.IsWalking)
        {
            if (Source.isPlaying && Source.clip == walking)
            {
                return;
            }

            Source.clip = walking;
            Source.Play();
        }
        else
        {
            Source.Stop();
        }
    }
}