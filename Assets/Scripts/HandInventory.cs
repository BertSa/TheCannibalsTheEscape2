using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInventory : MonoBehaviour
{
    [Header("Torch")] //Items player has in his inventory (Weapon is WIP)
    public GameObject torch;


    private void Start()
    {
        torch.SetActive(true);
    }
}