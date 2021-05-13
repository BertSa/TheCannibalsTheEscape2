using System;
using UnityEngine;

public class AreaStrategy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Cannibal") && CannibalsManager.IsInitialized)
            CannibalsManager.Instance.SetState(CannibalsManager.CannibalsState.Following);
    }
}