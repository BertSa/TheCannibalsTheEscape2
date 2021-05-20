using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AreaStrategy : Singleton<AreaStrategy>
{
    private Transform _player;
    [HideInInspector] public CapsuleCollider colliderPlayer;

    private void Start()
    {
        _player = PlayerController.Instance.transform;
        colliderPlayer = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        if (
            // !CannibalsManager.IsInitialized || 
            CannibalsManager.Instance.GetState() == CannibalsManager.CannibalsState.Searching ||GameManager.Instance.gameState!=GameManager.GameState.Playing)
            return;
        
        var colliders = new Collider[30];
        var positions = new List<Vector3>();

        if (Physics.OverlapSphereNonAlloc(_player.position, GetRadius(), colliders) > 0)
            positions.AddRange(from c in colliders where c != null && c.CompareTag("Cannibal") select c.transform.position);

        CannibalsManager.Instance.SetState(positions.Count == 0
            ? CannibalsManager.CannibalsState.Searching
            : CannibalsManager.CannibalsState.Following);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (
            // !GameManager.IsInitialized || 
            // !CannibalsManager.IsInitialized || 
            GameManager.Instance.gameState != GameManager.GameState.Playing ) return;
        
        if (other.gameObject.CompareTag("Cannibal") 
            // && CannibalsManager.IsInitialized
            )
            CannibalsManager.Instance.SetState(CannibalsManager.CannibalsState.Following);
    }

    public float GetRadius()
    {
        return colliderPlayer.radius * 2;
    }
}