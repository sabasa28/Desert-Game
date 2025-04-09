using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] List<Item> items = new List<Item>();
    [SerializeField] Item[] startingItems;
    [SerializeField] Item[] allPosibleItems;

    private void Start()
    {
        foreach (Item item in startingItems)
        {
            items.Add(item);
        }
    }
    public void AddItem(string itemToAdd)
    {
        foreach (Item item in allPosibleItems)
        {
            if (item.itemName == itemToAdd)
            {
                items.Add(item);
            }
        }
    }
    public void RemoveItem(string itemToRemove)
    {
        foreach (Item item in items)
        {
            if (item.itemName == itemToRemove)
            { 
                items.Remove(item);
                break;
            }
        }
    }
}
