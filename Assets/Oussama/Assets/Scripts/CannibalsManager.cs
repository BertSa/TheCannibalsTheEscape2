
using UnityEngine;

public class CannibalsManager : Singleton<CannibalsManager>
{

    private EnemyFollow[] canibals;
    private Transform player;
    private CannibalsState currentCannibalsState;
    
    protected override void Awake()
    {
        base.Awake();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        canibals = FindObjectsOfType<EnemyFollow>();
        player = PlayerController.Instance.GetComponent<Transform>();
        currentCannibalsState = CannibalsState.FOLLOWING;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var c in canibals)
        {
            var cannibalPosition = c.transform;
            Vector3 dirFromAtoB = (player.transform.position - cannibalPosition.position).normalized;
            float dotProd = Vector3.Dot(dirFromAtoB, cannibalPosition.forward);

            var lookingAtPlayer = dotProd >= 0 && dotProd <= 1;
            
            if(lookingAtPlayer)
            {
                currentCannibalsState = CannibalsState.FOLLOWING;
                return;
            }
            
            if (c.IsNearPlayer(1, EnemyFollow.DistanceToAttack))
            {
                currentCannibalsState = CannibalsState.ATTACKING;
                return;
            }
            
            currentCannibalsState = CannibalsState.SEARCHING;
            return;
        }
    }


    private enum CannibalsState
    {
        FOLLOWING,
        SEARCHING,
        ATTACKING
    }
}
