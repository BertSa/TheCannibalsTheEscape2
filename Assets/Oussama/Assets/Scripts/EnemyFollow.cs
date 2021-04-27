using UnityEngine;
using UnityEngine.AI;
using static GameManager;

public class EnemyFollow : MonoBehaviour
{
    [SerializeField] private int speed = 20;
    private NavMeshAgent _self;
    private Transform player;

    private void Start()
    {
        _self = GetComponent<NavMeshAgent>();
        player = PlayerController.Instance.GetComponent<Transform>();

        _self.acceleration = speed;
        _self.autoRepath = true;
    }

    private void Update()
    {
        _self.SetDestination(player.position);
    }

    private void FixedUpdate()
    {
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && GameManager.IsInitialized)
            GameManager.Instance.EndGame(EndingStatus.LostZombies);
    }
}