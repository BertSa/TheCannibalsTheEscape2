using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager.GameState;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector] public GameState gameState = Beginning;
    public Events.EventGameState onGameStateChanged = new Events.EventGameState();
    private bool _pausable = true;
    
    protected override void Awake()
    {
        base.Awake();
        Time.timeScale = 0;
    }

    private void Start()
    {
        var potentialExits = FindObjectsOfType<PotentialExit>();
        if (potentialExits.Length < 1) return;
        potentialExits[Random.Range(0, potentialExits.Length)].SetAsExit(true);
        SetGameState(Beginning);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && _pausable)
            TogglePause();
    }

    public void SetGameState(GameState actual)
    {
        Time.timeScale = actual == Playing ? 1 : 0;
        
        onGameStateChanged.Invoke(gameState, actual);
        
        gameState = actual;
        
        if (gameState != Beginning && gameState != Playing && gameState != Pause)
            _pausable = false;
    }

    private void TogglePause()
    {
        SetGameState(gameState == Playing ? Pause : Playing);
    }

    public enum GameState
    {
        Playing,
        LostCannibals,
        LostTorch,
        Won,
        Pause,
        Beginning
    }

    public static void RestartGame()
    {
        SceneManager.LoadScene("BootMenu");
    }
}