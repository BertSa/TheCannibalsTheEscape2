using Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public GameState State { get; private set; } = GameState.Beginning;
    public readonly Events.EventGameState OnGameStateChanged = new();
    private bool Pausable { get; set; } = true;

    protected override void Awake()
    {
        base.Awake();
        Time.timeScale = 0;
    }

    private void Start()
    {
        var potentialExits = FindObjectsOfType<PotentialExit>();
        if (potentialExits.Length < 1)
        {
            return;
        }

        var selectedExit = Random.Range(0, potentialExits.Length);
        potentialExits[selectedExit].SetAsExit(true);

        SetGameState(GameState.Beginning);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && Pausable)
        {
            TogglePause();
        }
    }

    public void SetGameState(GameState actual)
    {
        Time.timeScale = actual == GameState.Playing ? 1 : 0;

        OnGameStateChanged.Invoke(State, actual);

        State = actual;

        if (State != GameState.Beginning && State != GameState.Playing && State != GameState.Pause)
        {
            Pausable = false;
        }
    }

    private void TogglePause()
    {
        SetGameState(State == GameState.Playing ? GameState.Pause : GameState.Playing);
    }

    public static void RestartGame()
    {
        SceneManager.LoadScene("BootMenu");
    }
}