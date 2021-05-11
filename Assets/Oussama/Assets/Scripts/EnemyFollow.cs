using System;
using UnityEngine;
using UnityEngine.AI;
using static GameManager;

public class EnemyFollow : MonoBehaviour
{
    public const int DistanceToAttack = 2;
    private const int Acceleration = 1;
    private readonly int _attack = Animator.StringToHash("Attack");
    
    [SerializeField] private int speed = 5;
    
    private NavMeshAgent _agent;
    private Transform _player;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.SetBool(_attack, false);
        _agent = GetComponent<NavMeshAgent>();
        _player = PlayerController.Instance.GetComponent<Transform>();

        _agent.acceleration = Acceleration;
        _agent.speed = speed;
        _agent.autoRepath = true;
        _agent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
    }

    private void Update()
    {
        var playerPosition = _player.position;

        if (IsNearPlayer(1.4f, DistanceToAttack)) transform.LookAt(playerPosition);

        _agent.SetDestination(playerPosition);

        _animator.SetBool(_attack, IsNearPlayer(1, DistanceToAttack));
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && GameManager.IsInitialized)
            GameManager.Instance.SetGameState(GameState.LostCannibals);
    }


    private void OnTriggerEnter(Collider other)
    {
    }


    public bool IsNearPlayer(float rateDistance, int distance)
    {
        if (rateDistance <= 0)
            throw new ArgumentOutOfRangeException(nameof(rateDistance) + " cannot be lower or equals than 0");
        
        if (distance <= 0)
            throw new ArgumentOutOfRangeException(nameof(distance) + " cannot be lower or equals than 0");
        
        return Vector3.Distance(transform.position, _player.position) <= distance * rateDistance;
    }
}