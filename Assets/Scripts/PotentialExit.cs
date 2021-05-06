﻿using UnityEngine;

public class PotentialExit : MonoBehaviour
{
    [SerializeField] private GameObject exit;
    [SerializeField] private GameObject deadEnd;

    private void Start()
    {
        SetAsExit(false);
    }

    public void SetAsExit(bool isExit)
    {
        exit.SetActive(isExit);
        deadEnd.SetActive(!isExit);
        GetComponent<Collider>().enabled = isExit;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.IsInitialized) GameManager.Instance.EndGame(GameManager.GameState.Won);
    }
}