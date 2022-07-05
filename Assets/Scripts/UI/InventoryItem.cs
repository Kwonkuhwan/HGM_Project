using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class InventoryItem
{ 
    public ItemData ItemData;
    public int stackSize;

    public InventoryItem(ItemData item)
    {
        ItemData = item;
        AddToStack();

    }

    public void AddToStack()
    {
        stackSize++;
    }
    public void RemoveFromStack()
    {
        stackSize--;
    }

}
