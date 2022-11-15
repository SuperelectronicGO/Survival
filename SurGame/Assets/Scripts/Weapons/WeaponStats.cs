using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;




[Serializable]


public class WeaponStats
{
    public bool isWeapon;
    public enum WeaponType
    {
        None,
        Axe,
        Pickaxe,
    }

    public enum statDisplayName
    {
        NoStats,
        Axe,
        Pickaxe,
        
    }
    public WeaponType weaponType;
    public statDisplayName statNames;
    public float damage;
    public powers[] weaponPowers;

    public string[] statPer()
    {
        switch (statNames)
        {
            case statDisplayName.Pickaxe:
            
                return new string[] { "Damage", "PickaxePower" };
             
            case statDisplayName.Axe:
                return new string[] { "Damage", "AxePower" };
            case statDisplayName.NoStats:
            default:
                return new string[] { "" };



        }
    }



}
[Serializable]
public class powers
{
    
        public string name;
        public float power;
    
};

