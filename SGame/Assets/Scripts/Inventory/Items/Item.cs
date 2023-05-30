using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Collections;
using Unity.Netcode;
[Serializable]

public class Item
{
    //REMEMBER TO UPDATE ITEM STRUCT IN ALL LOCATIONS!
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
        OakStick,
        [InspectorName("Magic/Runes/FireballRune")]
        FireballRune,
        [InspectorName("Magic/Runes/VoidFireballRune")]
        VoidFireballRune



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
            case ItemType.FireballRune:
                return "Fireball Rune";
            case ItemType.VoidFireballRune:
                return "Void Fireball Rune";
                
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
            case ItemType.FireballRune:
                return "A rune that grants access to the fireball spell";
            case ItemType.VoidFireballRune:
                return "A rune that grants access to the void fireball spell";
                
                

        }
    }
    public int MaxStack()
    {
        switch (itemType)
        {
            default:
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
            case ItemType.FireballRune:
            case ItemType.VoidFireballRune:
                return false;
        }
    }


    public Spell spell;
    //For random textures

    private int tex;
    private bool generatedTex = false;
    
    //Spell reference, if it has one
      


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
            case ItemType.FireballRune: return ItemAssets.Instance.fireGrenadeRuneSprite;
            case ItemType.VoidFireballRune: return ItemAssets.Instance.voidFireballRuneSprite;
            
                

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
//Class holding Item conversions to structs, and vice versa
public static class ItemConversion{
    ///<summary>
    ///Method that takes an Item class and returns it as a struct
    ///</summary>
    ///<returns> A copy of the class in struct form</returns>
    public static ItemNetworkStruct ToStruct(this Item source)
    {
        if (source == null) throw new NullReferenceException("Attempting to convert a null object");
        //Keep this - otherwise spell is null if its not a rune
        if (source.spell == null)
        {
            source.spell = new Spell();
        }
        //Define a new itemStruct

        ItemNetworkStruct itemStruct = new ItemNetworkStruct
        {
            type = source.itemType,
            amount = source.amount,
            attributeNames = new ItemAttribute.AttributeName[source.attributes.Count],
            attributeValues = new float[source.attributes.Count],
            attributeInfo = "",
            spellType = new Spell.SpellType(),
            spellAttributeTypes = new SpellAttribute.AttributeType[source.spell.attributes.Count],
            spellAttributeValues = new float[source.spell.attributes.Count]
        };



        //Copy over item attribute values
        for (int i = 0; i < source.attributes.Count; i++)
        {
            itemStruct.attributeNames[i] = source.attributes[i].attribute;
            itemStruct.attributeValues[i] = source.attributes[i].value;
            string addedString = source.attributes[i].info + " <end> ";
            if ((itemStruct.attributeInfo + addedString).Length > 511) { throw new IndexOutOfRangeException("Attribute '" + source.attributes[i].attribute.ToString() + "' exceeds the 512 byte limit"); }
            itemStruct.attributeInfo += addedString;

        }
        //Copy over spell values
        itemStruct.spellType = source.spell.type;
        for (int i = 0; i < source.spell.attributes.Count; i++)
        {
            itemStruct.spellAttributeTypes[i] = source.spell.attributes[i].attribute;
            itemStruct.spellAttributeValues[i] = source.spell.attributes[i].value;
        }
        return itemStruct;
    }
    ///<summary>
    ///Method that takes an Item struct and returns it as a class
    ///</summary>
    ///<returns>A copy of the struct in class form</returns>
    public static Item ToClass(this ItemNetworkStruct source)
    {
        Item item = new Item();
        item.itemType = source.type;
        item.amount = source.amount;
        item.attributes = new List<ItemAttribute>();
        item.spell = new Spell();
        item.spell.type = source.spellType;
        //Copy item attributes from the struct
        string[] attributeInfos = source.attributeInfo.ToString().Split(new String[] { " <end> " }, StringSplitOptions.None);
        //Copy over Item and Spell attributes if their corresponding arrays are not null.
        if (source.attributeNames != null)
        {
            for (int i = 0; i < source.attributeNames.Length; i++)
            {
                item.attributes.Add(new ItemAttribute
                {
                    attribute = source.attributeNames[i],
                    value = source.attributeValues[i],
                    info = attributeInfos[i]
                });
            }
        }
        //Copy spell attributes from the struct
        item.spell.attributes = new List<SpellAttribute>();
        if (source.spellAttributeTypes != null)
        {
            for (int i = 0; i < source.spellAttributeTypes.Length; i++)
            {

                item.spell.attributes.Add(new SpellAttribute
                {
                    attribute = source.spellAttributeTypes[i],
                    value = source.spellAttributeValues[i]
                });
            }
        }
        return item;
    }
}
[Serializable]
public struct ItemNetworkStruct : INetworkSerializable
{
    public Item.ItemType type;
    public int amount;
    //Attribute values
    public ItemAttribute.AttributeName[] attributeNames;
    public float[] attributeValues;
    public NetworkString512Bytes attributeInfo;
    //Spell values
    public Spell.SpellType spellType;
    public SpellAttribute.AttributeType[] spellAttributeTypes;
    public float[] spellAttributeValues;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref type);
        serializer.SerializeValue(ref amount);
        serializer.SerializeValue(ref attributeNames);
        serializer.SerializeValue(ref attributeValues);
        serializer.SerializeValue(ref spellType);
        serializer.SerializeValue(ref spellAttributeTypes);
        serializer.SerializeValue(ref spellAttributeValues);
        //Check to make sure value isn't null as we are using unmanaged types
       // if (attributeInfos == null) { throw new NullReferenceException(); }
        serializer.SerializeValue(ref attributeInfo);
    }

}
[Serializable]
public struct NetworkString512Bytes : INetworkSerializable, IEquatable<NetworkString512Bytes>
{
    private ForceNetworkSerializeByMemcpy<FixedString512Bytes> data;

    public bool Equals(NetworkString512Bytes other)
    {
        throw new NotImplementedException();
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref data);
    }

    public override string ToString() => data.Value.ToString();

    public static implicit operator string(NetworkString512Bytes networkString) => networkString.ToString();
    public static implicit operator NetworkString512Bytes(string s) => new NetworkString512Bytes { data = new FixedString512Bytes(s) };
}
[Serializable]
public class ItemAttribute {

    public enum AttributeName
    {
        Stackable,
        Durability,
        MaxDurability,
        EnablesBuilding,
        Damage,
        Type, 
        AllowsSpell,

    }
    public AttributeName attribute;
    public float value;
    public string info;

}

[Serializable]
public class dropItem
{
    public GameObject itemPrefab;
    public Item item;
    public float chance;
    public Vector2 amount;
}






