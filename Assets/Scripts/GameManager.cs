﻿using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    void Start()
    {
    }

    void Update()
    {
    }

    public void EndGame()
    {
        Time.timeScale = 0;
    }
}