using UnityEngine;

public class PotentialExit : MonoBehaviour
{
    [SerializeField] private GameObject exit, deadEnd;

    private void Start()
    {
        SetAsExit(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.IsInitialized) GameManager.Instance.SetGameState(GameManager.GameState.Won);
    }

    public void SetAsExit(bool isExit)
    {
        exit.SetActive(isExit);
        deadEnd.SetActive(!isExit);
        GetComponent<Collider>().enabled = isExit;
    }
}