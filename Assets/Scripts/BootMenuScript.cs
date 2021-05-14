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
        
        start.onClick.AddListener(StartApplication);
        quit.onClick.AddListener(QuitApplication);
    }

    private static void QuitApplication()
    {
        Application.Quit();
    }

    protected virtual void StartApplication()
    {
        SceneManager.LoadScene("Oussama");
    }
}
