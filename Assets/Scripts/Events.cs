using System;
using UnityEngine.Events;

[Serializable]
public class Events
{
    public class EventGameState : UnityEvent<GameManager.GameState, GameManager.GameState>
    {}
    
    public class EventAmbiance : UnityEvent<CannibalsManager.CannibalsState, CannibalsManager.CannibalsState>
    {}
    
}