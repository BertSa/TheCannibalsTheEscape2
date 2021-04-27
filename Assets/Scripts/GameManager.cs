using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{
    private void Start()
    {
        var potentialExits = FindObjectsOfType<PotentialExit>();
        if (potentialExits.Length < 1) return;

        potentialExits[Random.Range(0, potentialExits.Length)].SetAsExit(true);
    }

    private void Update()
    {
    }

    // ReSharper disable once MemberCanBeMadeStatic.Global
    public void EndGame(EndingStatus status)
    {
        switch (status)
        {
            case EndingStatus.LostTorch:
                break;
            case EndingStatus.WinExit:
                break;
            case EndingStatus.LostZombies:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }

        Time.timeScale = 0;
    }

    public enum EndingStatus
    {
        LostZombies,
        LostTorch,
        WinExit
    }
}