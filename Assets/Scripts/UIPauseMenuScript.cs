using Enums;
using UnityEngine.SceneManagement;

public class UIPauseMenuScript : UIBootMenuScript
{
    protected override void Begin()
    {
        GameManager.Instance.UpdateGameState(GameState.Playing);
    }

    protected override void Exit()
    {
        SceneManager.LoadScene("BootMenu");
    }
}
