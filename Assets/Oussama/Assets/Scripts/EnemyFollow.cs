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
    }

    private void Update()
    {
        var playerPosition = _player.position;

        _agent.SetDestination(playerPosition);
        
        transform.rotation = Quaternion.LookRotation(playerPosition - transform.position);

        _animator.SetBool(_attack, IsNearPlayer(1, DistanceToAttack));
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && GameManager.IsInitialized)
            GameManager.Instance.SetGameState(GameState.LostCannibals);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("areaPlayer"))
        {
        }
    }

    private bool IsPlayerInArea()
    {
        const int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];
        var numColliders = Physics.OverlapSphereNonAlloc(transform.position, 50, hitColliders);
        for (var i = 0; i < numColliders; i++)
        {
            hitColliders[i].SendMessage(nameof(IsNearPlayer));
        }
        return false;
    }
    
    public void MoveToward( Vector3 targetPoint)
    {
        Quaternion rotation = Quaternion.LookRotation(targetPoint - transform.position);
        rotation.x = 0f;
        rotation.z = 0f;
        
        Transform transform1 = transform;
        
        transform1.rotation = Quaternion.Slerp(transform1.rotation, rotation, Time.deltaTime * _agent.angularSpeed);
 
        Vector3 movementVelocity = transform1.forward * speed;
 
        movementVelocity.y = -.08f;
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