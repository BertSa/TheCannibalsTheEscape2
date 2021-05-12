using UnityEngine;

public class AreaStrategy : MonoBehaviour
{
    void Start()
    {
    }
    
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Zombie") && CannibalsManager.IsInitialized)
            CannibalsManager.Instance.SetState(CannibalsManager.CannibalsState.Following);
    }
}