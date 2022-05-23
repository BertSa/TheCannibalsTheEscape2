using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;

public class AreaStrategy : Singleton<AreaStrategy>
{
    private Transform _player;
    [HideInInspector] public CapsuleCollider colliderPlayer;

    private bool IsPlaying => GameManager.Instance.State == GameState.Playing;

    protected override void Awake()
    {
        base.Awake();
        colliderPlayer = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        _player = PlayerController.Instance.transform;
    }

    private void Update()
    {
        if (!IsPlaying)
        {
            return;
        }

        if (CannibalsManager.Instance.State == CannibalsState.Searching)
        {
            return;
        }

        var colliders = new Collider[30];
        var positions = new List<Vector3>();

        if (Physics.OverlapSphereNonAlloc(_player.position, GetRadius(), colliders) > 0)
        {
            positions.AddRange(from c in colliders where c != null && c.CompareTag("Cannibal") select c.transform.position);
        }

        CannibalsManager.Instance.UpdateState(positions.Count == 0 ? CannibalsState.Searching : CannibalsState.Following);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlaying)
        {
            return;
        }

        if (other.gameObject.CompareTag("Cannibal"))
        {
            CannibalsManager.Instance.UpdateState(CannibalsState.Following);
        }
    }

    public float GetRadius()
    {
        return colliderPlayer.radius * 2;
    }
}