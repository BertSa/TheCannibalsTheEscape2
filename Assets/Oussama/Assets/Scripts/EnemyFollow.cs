using UnityEngine;
using UnityEngine.AI;
using static GameManager;
public class EnemyFollow : MonoBehaviour
{
    [SerializeField] private int speed = 20;
    private NavMeshAgent _self;
    [SerializeField] private Transform player;
    
    private void Start()
    {
        _self = GetComponent<NavMeshAgent>();
        _self.acceleration = speed;
        _self.SetDestination(player.position);
        _self.autoRepath = true;
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Player"))
            GameManager.Instance.EndGame(EndingStatus.LostZombies);
    }
}