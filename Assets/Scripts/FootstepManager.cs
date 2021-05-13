using UnityEngine;
using static GameManager.GameState;

public class FootstepManager : MonoBehaviour
{
    [Header("AudioSource")] [SerializeField]
    private AudioSource audioSource;

    [Header("Clips")] [SerializeField] private AudioClip running;

    [SerializeField] private AudioClip walking;
    private PlayerController player;

    private void Start()
    {
        if (PlayerController.IsInitialized) player = PlayerController.Instance;
    }

    private void Update()
    {
        if (!player.isSprinting && !player.isWalking && !audioSource.isPlaying||GameManager.Instance.gameState!=Playing) return;
        if (player.isSprinting)
        {
            if (audioSource.isPlaying && audioSource.clip == running)
                return;
            audioSource.clip = running;
            audioSource.Play();
        }
        else if (player.isWalking)
        {
            if (audioSource.isPlaying && audioSource.clip == walking)
                return;
            audioSource.clip = walking;
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }
}