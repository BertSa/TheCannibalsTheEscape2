using Enums;
using UnityEngine;

public class PotentialExit : MonoBehaviour
{
    [SerializeField] private GameObject exit;
    [SerializeField] private GameObject deadEnd;

    private Collider Collider { get; set; }

    private void Awake()
    {
        Collider = GetComponent<Collider>();
        SetAsExit(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.IsInitialized)
        {
            GameManager.Instance.UpdateGameState(GameState.Won);
        }
    }

    public void SetAsExit(bool isExit)
    {
        exit.SetActive(isExit);
        deadEnd.SetActive(!isExit);
        Collider.enabled = isExit;
    }
}