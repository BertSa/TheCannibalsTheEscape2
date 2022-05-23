using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BootMenuScript : MonoBehaviour
{
    [SerializeField] private Button start;
    [SerializeField] private Button quit;

    private void Start()
    {
        start.onClick = new Button.ButtonClickedEvent();
        quit.onClick = new Button.ButtonClickedEvent();

        start.onClick.AddListener(Begin);
        quit.onClick.AddListener(Exit);
    }

    protected virtual void Exit()
    {
        Application.Quit();
    }

    protected virtual void Begin()
    {
        SceneManager.LoadScene("Game");
    }
}