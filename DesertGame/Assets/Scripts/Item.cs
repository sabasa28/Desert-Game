using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite art;

    public Item(string newName, Sprite newArt)
    {
        itemName = newName;
        art = newArt;
    }
}
