using System;
using Enums;
using UnityEngine.Events;

[Serializable]
public class Events
{
    public class EventGameState : UnityEvent<GameState, GameState>
    {}
    
    public class EventAmbiance : UnityEvent<CannibalsState, CannibalsState>
    {}
    
}