using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;
using static GameManager.GameState;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Cinematic cinematicBeginning, 
        cinematicEndWin, 
        cinematicEndLostTorch, 
        cinematicEndLostCannibals;
    
    [SerializeField] private GameObject hud, pauseMenu;

    private void Start()
    {
        GameManager.Instance.onGameStateChanged.AddListener(HandleGameStateChanged);
    }

    private void HandleGameStateChanged(GameState previous, GameState actual)
    {
        switch (actual)
        {
            case Beginning:
                cinematicBeginning.gameObject.SetActive(true);
                break;
            case Pause:
                hud.SetActive(false);
                pauseMenu.SetActive(true);
                break;
            case Playing:
                pauseMenu.SetActive(false);
                if (previous == Beginning) cinematicBeginning.gameObject.SetActive(false);
                hud.SetActive(true);
                break;
            case Won:
                cinematicEndWin.gameObject.SetActive(true);
                break;
            case LostCannibals:
                cinematicEndLostCannibals.gameObject.SetActive(true);
                break;
            case LostTorch:
                cinematicEndLostTorch.gameObject.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actual), actual, null);
        }
    }

    public void CinematicFinished(Cinematic cinematic)
    {
        if (cinematic.Equals(cinematicBeginning))
            GameManager.Instance.SetGameState(Playing);
        else if (cinematic.Equals(cinematicEndWin) ||
                 cinematic.Equals(cinematicEndLostCannibals) ||
                 cinematic.Equals(cinematicEndLostTorch))
            RestartGame();
    }
}