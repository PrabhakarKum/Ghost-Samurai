using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldItemDatabase : MonoBehaviour
{
    public static WorldItemDatabase instance;
    public WeaponItems unarmedWeapons;
    
    [Header("Weapons")]
    public List<WeaponItems> weapons = new List<WeaponItems>();
    
    [Header("Items")]
    private List<Item> items = new List<Item>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //add all of our weapons to the list of items
        foreach (var weapon in weapons)
        {
            items.Add(weapon);
        }

        //Assign all our items in a unique id
        for (int i = 0; i < items.Count; i++)
        {
            items[i].itemID = i;
        }
    }

    public WeaponItems GetWeaponByID(int ID)
    {
        return weapons.FirstOrDefault(weapons => weapons.itemID == ID);
    }
}
