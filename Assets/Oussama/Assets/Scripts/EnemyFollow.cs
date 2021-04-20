using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

public class EnemyFollow : MonoBehaviour
{
    private const int Speed = 20;
    private NavMeshAgent self;
    [SerializeField] private Transform player;

    void Start()
    {
        self = GetComponent<NavMeshAgent>();
        self.acceleration = Speed;
        self.SetDestination(player.position);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        //TODO: make collision between cannibal and player lose the game...
    }
}
