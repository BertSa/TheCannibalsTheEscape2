
using Enums;
using UnityEngine.SceneManagement;

public class PauseMenuScript : BootMenuScript
{
    protected override void Begin()
    {
        GameManager.Instance.SetGameState(GameState.Playing);
    }

    protected override void Exit()
    {
        SceneManager.LoadScene("BootMenu");
    }
}
