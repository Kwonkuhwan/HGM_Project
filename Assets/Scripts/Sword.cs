using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sword : MonoBehaviour //, ICollectible
{
    public static event HandleSwordCollected OnSwordCollected;
    public delegate void HandleSwordCollected(ItemData itemData);

    public ItemData swordData;


    public void Collect()
    {
        Destroy(gameObject);
        OnSwordCollected?.Invoke(swordData); 
    }

}
