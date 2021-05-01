﻿using System;
using UnityEngine;
using UnityEngine.AI;
using static GameManager;

public class EnemyFollow : MonoBehaviour
{
    [SerializeField] private int speed = 5;
    private NavMeshAgent _agent;
    private Transform _player;
    private Animator _animator;
    private readonly int _attack = Animator.StringToHash("Attack");
    private const int DistanceToAttack = 6;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.SetBool(_attack, false);
        _agent = GetComponent<NavMeshAgent>();
        _player = PlayerController.Instance.GetComponent<Transform>();

        _agent.acceleration = speed;
        _agent.autoRepath = true;
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
            GameManager.Instance.EndGame(EndingStatus.LostZombies);
    }
}