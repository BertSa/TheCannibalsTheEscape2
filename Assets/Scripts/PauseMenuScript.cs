using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour
{
    [SerializeField] private Button start;
    [SerializeField] private Button quit;
    
    public void StartApplication()
    {
        Debug.Log(nameof(StartApplication));
        GameManager.Instance.SetGameState(GameManager.GameState.Playing);
    }
    
    private void Start()
    {
        start.onClick = new Button.ButtonClickedEvent();
        quit.onClick = new Button.ButtonClickedEvent();
    
        start.onClick.AddListener(StartApplication);
        quit.onClick.AddListener(QuitApplication);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

}
