using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;
using System.Linq;
public static class Extensions
{
    #region Item extensions
    ///<summary>
    ///Method that takes an Item class and returns it as a struct
    ///</summary>
    ///<returns> A copy of the class in struct form</returns>
    public static ItemNetworkStruct ToStruct(this Item source)
    {
        #region Exception checks
        //Throw exception if source is null
        if (source == null) throw new NullReferenceException("Attempting to convert a null object");
        //Define a new spell
        if (source.spell == null)
        {
            source.spell = new Spell();
        }
        //Throw exception if amount out of range
        if(source.amount > 255)
        {
            throw new ArgumentOutOfRangeException("Amount is larger than byte maximum value (255)");
        }
        #endregion

        #region Value copying
        //Sort array
        source.spell.attributes.Sort((x, y) => x.attribute.SpellAttributeIDFromType().CompareTo(y.attribute.SpellAttributeIDFromType()));
        source.attributes.Sort((x, y) => x.attribute.AttributeIDFromType().CompareTo(y.attribute.AttributeIDFromType()));
        ItemNetworkStruct itemStruct = new ItemNetworkStruct
        {
            type = source.itemType.IDFromItemType(),
            amount = (byte)source.amount,
            attributeNames = 0,
            attributeValues = new NetworkHalf[source.attributes.Count],
            spellType = 0,
            spellAttributeTypes = 0,
            spellAttributeValues = new NetworkHalf[source.spell.attributes.Count]
        };
        //Create short of attributes
        itemStruct.attributeNames = source.attributes.AttributeShortFromNameList();
        itemStruct.attributeValues = source.attributes.AttributeValuesFromClass();
        itemStruct.spellType = source.spell.type.SpellIDFromType();
        itemStruct.spellAttributeTypes = source.spell.attributes.SpellAttributeTypeUShortFromAttributeList();
        itemStruct.spellAttributeValues = source.spell.attributes.SpellAttributeValuesFromClass();
        for (int i = 0; i < source.spell.attributes.Count; i++)
        {
            itemStruct.spellAttributeValues[i] = source.spell.attributes[i].value;
        }
        #endregion
        return itemStruct;
    }
    ///<summary>
    ///Method that takes an Item struct and returns it as a class
    ///</summary>
    ///<returns>A copy of the struct in class form</returns>
    public static Item ToClass(this ItemNetworkStruct source)
    {
        Item item = new Item();
        item.itemType = source.type.ItemTypeFromID();
        item.amount = source.amount;
        item.attributes = new List<ItemAttribute>();
        item.spell = new Spell();
        item.spell.type = source.spellType.SpellTypeFromID();
        //Copy item attributes from the struct
        //Copy Item attributes
        ItemAttribute.AttributeName[] attributeNames = source.attributeNames.AttributeNameFromUShort();
        float[] attributeValues = source.attributeValues.AttributeValuesFromStruct();
            for (int i = 0; i < source.attributeValues.Length; i++)
            {
                item.attributes.Add(new ItemAttribute
                {
                    attribute = attributeNames[i],
                    value = attributeValues[i],
                });
            }
        
        //Copy spell attributes from the struct
        item.spell.attributes = new List<SpellAttribute>();
        SpellAttribute.AttributeType[] spellAttributeTypes = source.spellAttributeTypes.SpellAttributeTypeFromUShort();
        float[] spellattributeValues = source.spellAttributeValues.SpellAttributeValuesFromStruct();
            for (int i = 0; i < source.spellAttributeValues.Length; i++)
            {

            item.spell.attributes.Add(new SpellAttribute
            {
                attribute = spellAttributeTypes[i],
                value = spellattributeValues[i]
            });
            }
        
        return item;
    }
    /// <summary>
    /// Method that returns the ItemType from an ID
    /// </summary>
    /// <param name="id">The id of the item</param>
    /// <returns>The ItemType</returns>
    public static Item.ItemType ItemTypeFromID(this short id)
    {
        switch (id)
        {
            default:
            case 0:
                return Item.ItemType.Blank;
            case 1:
                return Item.ItemType.Campfire;
            case 2:
                return Item.ItemType.Firepit;
            case 3:
                return Item.ItemType.RockWall;
            case 4:
                return Item.ItemType.StakeWall;
            case 5:
                return Item.ItemType.AshLog;
            case 6:
                return Item.ItemType.BeechLog;
            case 7:
                return Item.ItemType.BirchLog;
            case 8:
                return Item.ItemType.OakLog;
            case 9:
                return Item.ItemType.SpruceLog;
            case 10:
                return Item.ItemType.StoneHatchet;
            case 11:
                return Item.ItemType.IronSword;
            case 12:
                return Item.ItemType.Shell;
            case 13:
                return Item.ItemType.ShellAxe;
            case 14:
                return Item.ItemType.LimestoneRock;
            case 15:
                return Item.ItemType.FieldstoneRock;
            case 16:
                return Item.ItemType.SlateRock;
            case 17:
                return Item.ItemType.OakStick;
            case 18:
                return Item.ItemType.FireballRune;
            case 19:
                return Item.ItemType.VoidFireballRune;

        }
    }
    /// <summary>
    /// Method that returns the ID from an ItemType
    /// </summary>
    /// <param name="type">The type of the item</param>
    /// <returns>The ID of the item</returns>
    public static short IDFromItemType(this Item.ItemType type)
    {
        switch (type)
        {
            default:
            case Item.ItemType.Blank:
                return 0;
            case Item.ItemType.Campfire:
                return 1;
            case Item.ItemType.Firepit:
                return 2;
            case Item.ItemType.RockWall:
                return 3;
            case Item.ItemType.StakeWall:
                return 4;
            case Item.ItemType.AshLog:
                return 5;
            case Item.ItemType.BeechLog:
                return 6;
            case Item.ItemType.BirchLog:
                return 7;
            case Item.ItemType.OakLog:
                return 8;
            case Item.ItemType.SpruceLog:
                return 9;
            case Item.ItemType.StoneHatchet:
                return 10;
            case Item.ItemType.IronSword:
                return 11;
            case Item.ItemType.Shell:
                return 12;
            case Item.ItemType.ShellAxe:
                return 13;
            case Item.ItemType.LimestoneRock:
                return 14;
            case Item.ItemType.FieldstoneRock:
                return 15;
            case Item.ItemType.SlateRock:
                return 16;
            case Item.ItemType.OakStick:
                return 17;
            case Item.ItemType.FireballRune:
                return 18;
            case Item.ItemType.VoidFireballRune:
                return 19;

        }
    }
    /// <summary>
    /// Method that returns an attribute name from its ID
    /// </summary>
    /// <param name="id">The id of the attribute</param>
    /// <returns>The attribute</returns>
    public static ItemAttribute.AttributeName AttributeNameFromID(byte id)
    {
        switch (id)
        {
            default:
                throw new NullReferenceException($"Id '{id}' points to an index that doesn't exist");
            case 0:
                return ItemAttribute.AttributeName.Stackable;
            case 1:
                return ItemAttribute.AttributeName.Durability;
            case 2:
                return ItemAttribute.AttributeName.MaxDurability;
            case 3:
                return ItemAttribute.AttributeName.EnablesBuilding;
            case 4:
                return ItemAttribute.AttributeName.Damage;
            case 5:
                return ItemAttribute.AttributeName.Type;
            case 6:
                return ItemAttribute.AttributeName.AllowsSpell;
        }
    }
    /// <summary>
    /// Method that returns an attribute ID from its name
    /// </summary>
    /// <param name="type">The type of the attribute</param>
    /// <returns>The attribute ID</returns>
    public static short AttributeIDFromType(this ItemAttribute.AttributeName type)
    {
        switch (type)
        {
            default:
                throw new NullReferenceException("Type not found");
            case ItemAttribute.AttributeName.Stackable:
                return 0;
            case ItemAttribute.AttributeName.Durability:
                return 1;
            case ItemAttribute.AttributeName.MaxDurability:
                return 2;
            case ItemAttribute.AttributeName.EnablesBuilding:
                return 3;
            case ItemAttribute.AttributeName.Damage:
                return 4;
            case ItemAttribute.AttributeName.Type:
                return 5;
            case ItemAttribute.AttributeName.AllowsSpell:
                return 6;
        }
    }
    /// <summary>
    /// Method that returns the list of attributes from a UShort
    /// </summary>
    /// <param name="attributes">The list of attributes (each bit represents an attribute)</param>
    /// <returns>An array of ItemAttributes</returns>
    public static ItemAttribute.AttributeName[] AttributeNameFromUShort(this ushort attributes)
    {
        //Create new attribute list
        List<ItemAttribute.AttributeName> attributeList = new List<ItemAttribute.AttributeName>();
        //Iterate through each bit
        for(byte i = 0; i < 16; i++)
        {
            if (attributes.GetBitFromUShort(i) == true)
            {
                attributeList.Add(AttributeNameFromID(i));
            }
        }
        //Return array
        return attributeList.ToArray();
    }
    /// <summary>
    /// Method that creates a UShort from an array of ItemAttribute Names
    /// </summary>
    /// <param name="source">The ItemAttribute array to create the short from</param>
    /// <returns>A Ushort where each bit represents an attribute</returns>
    public static ushort AttributeShortFromNameList(this List<ItemAttribute> source)
    {
        ushort attributes = 0;
        for(int i = 0; i < source.Count; i++)
        {
            attributes = attributes.SetBitAtUShort(source[i].attribute.AttributeIDFromType());
        }
        return attributes;
    }
    /// <summary>
    /// Method that converts the half[] from an ItemNetworkStruct to a float[] for the class
    /// </summary>
    /// <param name="source">The half[] to reference</param>
    /// <returns>A float[] of values</returns>
    public static float[] AttributeValuesFromStruct(this NetworkHalf[] source)
    {
        float[] values = new float[source.Length];
        for(int i = 0; i < source.Length; i++)
        {
            values[i] = source[i].data.Value;
            
        }
        return values;
    }
    /// <summary>
    /// Method that converts the float[] from an Item class to a half[] for the ItemNetworkStruct
    /// </summary>
    /// <param name="source">The float[] to reference</param>
    /// <returns>A half[] of values</returns>
    public static NetworkHalf[] AttributeValuesFromClass(this List<ItemAttribute> source)
    {
        NetworkHalf[] values = new NetworkHalf[source.Count];
        for(int i = 0; i < source.Count; i++)
        {
            values[i] = new NetworkHalf();
            values[i] = source[i].value;
        }
        return values;
    }
    /// <summary>
    /// Method that returns the associated SpellType from an ID
    /// </summary>
    /// <param name="id">The ID of the spell</param>
    /// <returns>The SpellType</returns>
    public static Spell.SpellType SpellTypeFromID(this byte id)
    {
        switch (id)
        {
            default:
                throw new NullReferenceException($"id '{id}' not found");
            case 0:
                return Spell.SpellType.None;
            case 1:
                return Spell.SpellType.Fireball;
            case 2: 
                return Spell.SpellType.VoidFireball;
        }
    }
    /// <summary>
    /// Method that returns the associated ID from a SpellType
    /// </summary>
    /// <param name="type">The type of the spell</param>
    /// <returns>The spell ID</returns>
    public static byte SpellIDFromType(this Spell.SpellType type)
    {
        switch (type)
        {
            default:
                throw new NullReferenceException("Spell doesn't have an assigned ID");
            case Spell.SpellType.None:
                return 0;
            case Spell.SpellType.Fireball:
                return 1;
            case Spell.SpellType.VoidFireball:
                return 2;

        }
    }
    /// <summary>
    /// Method that gets a SpellAttribute.AttributeType from its ID
    /// </summary>
    /// <param name="id">The id of the attribute</param>
    /// <returns>The attribute from its id</returns>
    public static SpellAttribute.AttributeType SpellAttributeNameFromID(byte id)
    {
        switch (id)
        {
            default:
                throw new NullReferenceException($"Id '{id}' points to an attribute name that doesn't exist");
            case 0:
                return SpellAttribute.AttributeType.chargeSpeed;
            case 1:
                return SpellAttribute.AttributeType.summonTime;
            case 2:
                return SpellAttribute.AttributeType.damage;
        }
    }
    /// <summary>
    /// Method that returns a spell attribute ID from its name
    /// </summary>
    /// <param name="type">The type of the attribute</param>
    /// <returns>The attribute ID</returns>
    public static short SpellAttributeIDFromType(this SpellAttribute.AttributeType type)
    {
        switch (type)
        {
            default:
                throw new NullReferenceException("Type not found");
            case SpellAttribute.AttributeType.chargeSpeed:
                return 0;
            case SpellAttribute.AttributeType.summonTime:
                return 1;
            case SpellAttribute.AttributeType.damage:
                return 2;
        }
    }
    /// <summary>
    /// Method that returns the list of spell attributes from a UShort
    /// </summary>
    /// <param name="attributes">The list of attributes (each bit represents an attribute)</param>
    /// <returns>An array of ItemAttributes</returns>
    public static SpellAttribute.AttributeType[] SpellAttributeTypeFromUShort(this ushort attributes)
    {
        //Create new attribute list
        List<SpellAttribute.AttributeType> attributeList = new List<SpellAttribute.AttributeType>();
        //Iterate through each bit
        for (byte i = 0; i < 16; i++)
        {
            if (attributes.GetBitFromUShort(i) == true)
            {
                attributeList.Add(SpellAttributeNameFromID(i));
            }
        }
        //Return array
        return attributeList.ToArray();
    }
    /// <summary>
    /// Method that returns the ushort of a spells attributes from the class attribute list
    /// </summary>
    /// <param name="source">The attribute list reference</param>
    /// <returns>A ushort of the spells attributes</returns>
    public static ushort SpellAttributeTypeUShortFromAttributeList(this List<SpellAttribute> source)
    {
        ushort attributes = 0;
        for (int i = 0; i < source.Count; i++)
        {
            attributes = attributes.SetBitAtUShort(source[i].attribute.SpellAttributeIDFromType());
        }
        return attributes;
    }
    /// <summary>
    /// Method that converts the half[] from an ItemNetworkStruct spell values to a float[] for the class
    /// </summary>
    /// <param name="source">The half[] to reference</param>
    /// <returns>A float[] of values</returns>
    public static float[] SpellAttributeValuesFromStruct(this NetworkHalf[] source)
    {
        float[] values = new float[source.Length];
        for (int i = 0; i < source.Length; i++)
        {
            values[i] = source[i].data.Value;

        }
        return values;
    }
    /// <summary>
    /// Method that converts the float[] from an Item class spell atttributes to a half[] for the ItemNetworkStruct
    /// </summary>
    /// <param name="source">The float[] to reference</param>
    /// <returns>A half[] of values</returns>
    public static NetworkHalf[] SpellAttributeValuesFromClass(this List<SpellAttribute> source)
    {
        NetworkHalf[] values = new NetworkHalf[source.Count];
        for (int i = 0; i < source.Count; i++)
        {
            values[i] = new NetworkHalf();
            values[i] = source[i].value;
        }
        return values;
    }
    /// <summary>
    /// Method that converts a spell in class form to its struct equivelent
    /// </summary>
    /// <param name="spell"></param>
    /// <returns>A copy of the spell in struct form</returns>
    public static SpellNetworkStruct SpellNetworkClassToStruct(this Spell source)
    {
        //Check if spell is null, and throw if it is
        if (source == null)
        {
            throw new System.NullReferenceException();
        }
        //Define a new SpellNetworkStruct
        SpellNetworkStruct spellStruct = new SpellNetworkStruct
        {
            type = 0,
            attributeTypes = 0,
            attributeValues = new NetworkHalf[source.attributes.Count]
        };

        spellStruct.type = source.type.SpellIDFromType();
        spellStruct.attributeTypes = source.attributes.SpellAttributeTypeUShortFromAttributeList();
        spellStruct.attributeValues = source.attributes.SpellAttributeValuesFromClass();
        //Return the final struct
        return spellStruct;
    }
    /// <summary>
    /// Method that converts a spell in struct form to its class equivelent
    /// </summary>
    /// <param name="spellStruct"></param>
    /// <returns>A copy of the spell struct as a class</returns>
    public static Spell SpellNetworkStructToClass(this SpellNetworkStruct source)
    {
        //Create a new spell
        Spell spell = new Spell();
        spell.type = source.type.SpellTypeFromID();
        spell.attributes = new List<SpellAttribute>();
        SpellAttribute.AttributeType[] spellAttributeTypes = source.attributeTypes.SpellAttributeTypeFromUShort();
        float[] spellattributeValues = source.attributeValues.SpellAttributeValuesFromStruct();
        for (int i = 0; i < source.attributeValues.Length; i++)
        {

            spell.attributes.Add(new SpellAttribute
            {
                attribute = spellAttributeTypes[i],
                value = spellattributeValues[i]
            });
        }
        //Return the spell
        return spell;
    }
    #endregion

    /// <summary>
    /// Method that checks the individual value of a bit in a UShort
    /// </summary>
    /// <param name="source">The UShort to check</param>
    /// <param name="bitNumber">The index of the bit to check</param>
    /// <returns></returns>
    public static bool GetBitFromUShort(this ushort source, int bitNumber)
    {
        return (source & (1 << bitNumber)) != 0;
    }
    /// <summary>
    /// Method that sets an individual bit in a UShort
    /// </summary>
    /// <param name="source">The ushort to modify</param>
    /// <param name="position">The position of the bit to modify</param>
    /// <returns></returns>
    public static ushort SetBitAtUShort(this ushort source, short position)
    {

        return (ushort)(source | (1 << position));
    }
}
