using UnityEngine;
using UnityEngine.AI;
using static GameManager;

public class EnemyFollow : MonoBehaviour
{
    private const int Acceleration = 1;
    [SerializeField] private int speed = 5;
    private NavMeshAgent _agent;
    private Transform _player;
    private Animator _animator;
    private readonly int _attack = Animator.StringToHash("Attack");
    private const int DistanceToAttack = 2;

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
        if (_agent.isOnNavMesh)
        {
            _agent.ResetPath();
            _agent.SetDestination(_player.position);
        }

        _animator.SetBool(_attack, Vector3.Distance(transform.position, _player.position) <= DistanceToAttack);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && GameManager.IsInitialized)
            GameManager.Instance.SetGameState(GameState.LostCannibals);
    }
}