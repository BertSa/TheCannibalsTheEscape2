using System;
using Enums;
using UnityEngine;
using static GameManager;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Cinematic cinematicBeginning;
    [SerializeField] private Cinematic cinematicEndWin;
    [SerializeField] private Cinematic cinematicEndLostTorch;
    [SerializeField] private Cinematic cinematicEndLostCannibals;

    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject pauseMenu;

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
    }

    private void HandleGameStateChanged(GameState previous, GameState actual)
    {
        switch (actual)
        {
            case GameState.Beginning:
                cinematicBeginning.gameObject.SetActive(true);
                break;
            case GameState.Pause:
                hud.SetActive(false);
                pauseMenu.SetActive(true);
                break;
            case GameState.Playing:
                hud.SetActive(true);
                pauseMenu.SetActive(false);
                if (previous == GameState.Beginning)
                {
                    cinematicBeginning.gameObject.SetActive(false);
                }
                break;
            case GameState.Won:
                cinematicEndWin.gameObject.SetActive(true);
                break;
            case GameState.LostCannibals:
                cinematicEndLostCannibals.gameObject.SetActive(true);
                break;
            case GameState.LostTorch:
                cinematicEndLostTorch.gameObject.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actual), actual, null);
        }
    }

    public void CinematicFinished(Cinematic cinematic)
    {
        if (cinematic.Equals(cinematicBeginning))
        {
            GameManager.Instance.SetGameState(GameState.Playing);
        }
        else if (cinematic.Equals(cinematicEndWin) || cinematic.Equals(cinematicEndLostCannibals) || cinematic.Equals(cinematicEndLostTorch))
        {
            RestartGame();
        }
    }
}