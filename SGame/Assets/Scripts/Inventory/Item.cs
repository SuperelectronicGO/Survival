using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]

public class Item
{

    public enum ItemType
    {

        [InspectorName("Blank")]
        Blank,
        [InspectorName("Buildings/Standalone/Campfire")]
        Campfire,
        [InspectorName("Buildings/Standalone/Firepit")]
        Firepit,
        [InspectorName("Buildings/Node/Rock Wall")]
        RockWall,
        [InspectorName("Buildings/Node/Stake Wall")]
        StakeWall,
        [InspectorName("Resources/Wood/Ash Log")]
        AshLog,
        [InspectorName("Resources/Wood/Beech Log")]
        BeechLog,
        [InspectorName("Resources/Wood/Birch Log")]
        BirchLog,
        [InspectorName("Resources/Wood/Oak Log")]
        OakLog,
        [InspectorName("Resources/Wood/Spruce Log")]
        SpruceLog,
        [InspectorName("Equipable/Tools/Stone Hatchet")]
        StoneHatchet,
        [InspectorName("Equipable/Melee Weapons/Iron Sword")]
        IronSword,
        [InspectorName("Resources/Natural/Shell")]
        Shell,
        [InspectorName("Equipable/Tools/Shell Axe")]
        ShellAxe,
        [InspectorName("Resources/Rock/Limestone Rock")]
        LimestoneRock,
        [InspectorName("Resources/Rock/Fieldstone Rock")]
        FieldstoneRock,
        [InspectorName("Resources/Rock/Slate Rock")]
        SlateRock,
        [InspectorName("Resources/Wood/Oak Stick")]
        OakStick

    }





    public ItemType itemType;
    public int amount;



    public string itemName()
    {
        switch (itemType)
        {
            case ItemType.Blank:
            default:
                return "Blank";
            case ItemType.Campfire:
                return "Campfire";
            case ItemType.Firepit:
                return "Firepit";
            case ItemType.RockWall:
                return "Rock wall";
            case ItemType.StakeWall:
                return "Stake wall";
            case ItemType.AshLog:
                return "Ash log";
            case ItemType.BeechLog:
                return "Beech Log";
            case ItemType.BirchLog:
                return "Birch Log";
            case ItemType.OakLog:
                return "Oak Log";
            case ItemType.SpruceLog:
                return "Spruce Log";
            case ItemType.StoneHatchet:
                return "Stone Hatchet";
            case ItemType.IronSword:
                return "Iron Sword";
            case ItemType.Shell:
                return "Shell";
            case ItemType.ShellAxe:
                return "Shell Axe";
            case ItemType.OakStick:
                return "Oak Stick";
            case ItemType.FieldstoneRock:
                return "Fieldstone Rock";
            case ItemType.LimestoneRock:
                return "Limestone Rock";
            case ItemType.SlateRock:
                return "Slate Rock";
        }
    }

    public string itemTooltip()
    {
        switch (itemType)
        {
            case ItemType.Blank:

            default:
                return "";
            case ItemType.Campfire:
                return "A Campfire";
            case ItemType.Firepit:
                return "Campfire with perks";
            case ItemType.RockWall:
                return "Sturdy wall offering decent protection";
            case ItemType.StakeWall:
                return "A budget protective option that is strong in numbers";
            case ItemType.AshLog:
                return "Strong yet flexible wood";
            case ItemType.BeechLog:
                return "Common wood with many uses";
            case ItemType.BirchLog:
                return "Useful wood with many perks";
            case ItemType.OakLog:
                return "Tough wood that doesn't break easily";
            case ItemType.SpruceLog:
                return "Uncommon wood sought after for its bendability";
            case ItemType.StoneHatchet:
                return "A simple yet useful gathering tool";
            case ItemType.IronSword:
                return "A solid means of defending ones self";
            case ItemType.LimestoneRock:
                return "A chunk of limestone";
            case ItemType.SlateRock:
                return "A chunk of slate";
            case ItemType.FieldstoneRock:
                return "A chunk of fieldstone";
            case ItemType.OakStick:
                return "A small oak stick";
            case ItemType.Shell:
                return "A small shell, once used to house a small creature";
            case ItemType.ShellAxe:
                return "An axe with a blade made of shells, sacrificing durability for strength";

        }
    }
    public int MaxStack()
    {
        switch (itemType)
        {

            case ItemType.Campfire:
            case ItemType.Firepit:
            case ItemType.StoneHatchet:
            case ItemType.IronSword:
            case ItemType.ShellAxe:

                return 1;


            case ItemType.RockWall:
            case ItemType.StakeWall:
                return 8;

            case ItemType.AshLog:
            case ItemType.BeechLog:
            case ItemType.BirchLog:
            case ItemType.OakLog:
            case ItemType.SpruceLog:
            case ItemType.Shell:
            case ItemType.LimestoneRock:
            case ItemType.FieldstoneRock:
            case ItemType.SlateRock:
            case ItemType.OakStick:
                return 20;


            case ItemType.Blank:
            default:
                return 6942069;
        }
    }


    public bool Stackable()
    {
        switch (itemType)
        {
            default:
            case ItemType.Blank:

                return true;
            case ItemType.Campfire:
            case ItemType.Firepit:
            case ItemType.StoneHatchet:
            case ItemType.IronSword:
            case ItemType.ShellAxe:
                return false;
        }
    }



    public GameObject getModel()
    {
        switch (itemType)
        {
            default:
            case ItemType.Campfire:
            case ItemType.RockWall:
            case ItemType.Firepit:
            case ItemType.StakeWall:
                return ItemAssets.Instance.bagModel;
            case ItemType.AshLog:
                return ItemAssets.Instance.ashLogModel;
            case ItemType.BeechLog:
                return ItemAssets.Instance.beechLogModel;
            case ItemType.BirchLog:
                return ItemAssets.Instance.birchLogModel;
            case ItemType.OakLog:
                return ItemAssets.Instance.oakLogModel;
            case ItemType.SpruceLog:
                return ItemAssets.Instance.spruceLogModel;
            case ItemType.StoneHatchet:
                return ItemAssets.Instance.stoneHatchetModel;
            case ItemType.IronSword:
                return ItemAssets.Instance.ironSwordModel;
            case ItemType.FieldstoneRock:
                return ItemAssets.Instance.fieldstoneRockModel;
            case ItemType.LimestoneRock:
                return ItemAssets.Instance.limestoneRockModel;
            case ItemType.SlateRock:
                return ItemAssets.Instance.slateRockModel;
            case ItemType.OakStick:
                return ItemAssets.Instance.oakStickModel;
            case ItemType.Shell:
                return ItemAssets.Instance.shellModel;
            case ItemType.ShellAxe:
                return ItemAssets.Instance.shellAxeModel;
        }
    }
    //For random textures

    private int tex;
    private bool generatedTex = false;
    

      


    public Sprite GetSprite()
    {
        switch (itemType)
        {
            default:
            case ItemType.Blank: return ItemAssets.Instance.blankSprite;
            case ItemType.AshLog: return ItemAssets.Instance.ashLogSprite;

            case ItemType.Campfire: return ItemAssets.Instance.campfireSprite;
            case ItemType.Firepit: return ItemAssets.Instance.firepitSprite;
            case ItemType.RockWall: return ItemAssets.Instance.rockWallSprite;
            case ItemType.StakeWall: return ItemAssets.Instance.stakeWallSprite;
            case ItemType.BeechLog: return ItemAssets.Instance.beechLogSprite;
            case ItemType.BirchLog: return ItemAssets.Instance.birchLogSprite;
            case ItemType.OakLog: return ItemAssets.Instance.oakLogSprite;
            case ItemType.SpruceLog: return ItemAssets.Instance.spruceLogSprite;
            case ItemType.StoneHatchet: return ItemAssets.Instance.stoneHatchetSprite;
            case ItemType.IronSword: return ItemAssets.Instance.ironSwordSprite;
            case ItemType.OakStick: return ItemAssets.Instance.oakStickSprite;
            case ItemType.LimestoneRock: return ItemAssets.Instance.limestoneRockSprite;
            case ItemType.SlateRock: return ItemAssets.Instance.slateRockSprite;
            case ItemType.FieldstoneRock: return ItemAssets.Instance.fieldstoneRockSprite;
            case ItemType.ShellAxe: return ItemAssets.Instance.shellAxeSprite;
            case ItemType.Shell:
                if (!generatedTex)
                {
                    tex = UnityEngine.Random.Range(1, 3);
                    generatedTex = true;
                }
                if (tex == 1)
                {
                    return ItemAssets.Instance.shellSpriteOne;
                }
                else
                {
                    return ItemAssets.Instance.shellSpriteTwo;
                }

                

        }
    }
    [NonReorderable]
    public List<ItemAttribute> attributes;

    public bool hasAttribute(ItemAttribute.AttributeName attribute)
    {
        foreach(ItemAttribute a in attributes)
        {
            if(a.attribute == attribute)
            {
                return true;
            }
        }
        return false;
    }

    public float getAttributeValue(ItemAttribute.AttributeName attribute)
    {
        foreach (ItemAttribute a in attributes)
        {
            if (a.attribute == attribute)
            {
                return a.value;
            }
        }
        Debug.LogError("No attribute of type found");
        return 0;
      
    }
    public string getAttributeString(ItemAttribute.AttributeName attribute)
    {
        foreach(ItemAttribute a in attributes)
        {
            if (a.attribute == attribute)
            {
                return a.info;
            }
        }
        Debug.LogError("No attribute of string type found");
        return "Missing";
    }
    public void setAttributeValue(ItemAttribute.AttributeName attribute, float value, string modifier)
    {

        foreach (ItemAttribute a in attributes)
        {
            if (a.attribute == attribute)
            {
                switch (modifier)
                {
                    case "+":
                        a.value += value;
                        return;
                    case "-":
                        a.value -= value;
                        return;
                    case "*":
                        a.value *= value;
                        return;
                    case "/":
                        a.value /= value;
                        return;
                    case "=":
                    default:
                        a.value = value;
                        return;
                       
                      
                }
            }
           
        }
        
    }













}
[Serializable]
public class ItemAttribute
{
    public enum AttributeName
    {
        Stackable,
        Durability,
        MaxDurability,
        EnablesBuilding,
        Damage,
        Type

    }
    public AttributeName attribute;
    public float value;
    public string info;
}





