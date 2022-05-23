using Enums;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    private const int DistanceToAttack = 4;
    private readonly int _attack = Animator.StringToHash("Attack");

    [SerializeField] private int speed = 3;
    public bool IsNearPlayer => Vector3.Distance(transform.position, Player.position) <= DistanceToAttack;

    private NavMeshAgent Agent { get; set; }
    private Transform Player { get; set; }
    private Animator Anim { get; set; }
    private Rigidbody Rb { get; set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        Anim = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        Player = PlayerController.Instance.GetComponent<Transform>();
        Anim.SetBool(_attack, false);

        Agent.speed = speed;
        Agent.autoRepath = true;
        Agent.autoBraking = true;
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing || CannibalsManager.Instance.State != CannibalsState.Following)
        {
            return;
        }

        SetDestination(Player.position);

        RotateToward(Agent.destination);

        Anim.SetBool(_attack, IsNearPlayer);
    }

    public void SetDestination(Vector3 destination) => Agent.SetDestination(destination);

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.UpdateGameState(GameState.LostCannibals);
        }
    }

    private void RotateToward(Vector3 targetPoint)
    {
        var t = transform;

        var rotation = Quaternion.LookRotation(targetPoint - t.position);
        rotation.x = 0f;
        rotation.z = 0f;


        t.rotation = Quaternion.Slerp(t.rotation, rotation, Time.deltaTime * Agent.angularSpeed);

        var movementVelocity = t.forward * speed;

        movementVelocity.y = -0.8f;

        Rb.velocity = movementVelocity;
    }
}