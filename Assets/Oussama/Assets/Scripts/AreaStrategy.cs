﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CannibalsManager.CannibalsState;

public class AreaStrategy : Singleton<AreaStrategy>
{
    private Transform _player;
    [HideInInspector] public CapsuleCollider colliderPlayer;

    private void Start()
    {
        if (PlayerController.IsInitialized) _player = PlayerController.Instance.transform;
        colliderPlayer = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        if (!CannibalsManager.IsInitialized || CannibalsManager.Instance.GetState() == Searching)
            return;
        
        var colliders = new Collider[100];
        var positions = new List<Vector3>();

        if (Physics.OverlapSphereNonAlloc(_player.position, GetRadius(), colliders) > 0)
            positions.AddRange(from c in colliders where c != null && c.CompareTag("Cannibal") select c.transform.position);

        if (positions.Count == 0)
            CannibalsManager.Instance.SetState(Searching);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Cannibal") && CannibalsManager.IsInitialized)
            CannibalsManager.Instance.SetState(Following);
    }

    public float GetRadius()
    {
        return colliderPlayer.radius * 2;
    }
}