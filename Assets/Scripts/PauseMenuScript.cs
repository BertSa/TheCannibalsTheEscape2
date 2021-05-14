
using UnityEngine.SceneManagement;

public class PauseMenuScript : BootMenuScript
{
    protected override void StartApplication()
    {
        GameManager.Instance.SetGameState(GameManager.GameState.Playing);
    }

    protected override void QuitApplication()
    {
        SceneManager.LoadScene("BootMenu");
    }
}
