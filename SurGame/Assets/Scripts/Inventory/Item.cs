using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]

public class Item
{
    public enum ItemType
    {
        Blank,
       Grenade,
        FireAxe,
        IronAxe,
        FieldstoneRock,
        SmallFieldstoneRock,
        Bone,
        DriedGrass,
        OakLog,
        PointedRock,
        SharpRock,
        SmallBone,
        OakStick,
        OakTwig,
        Twine,
        Acorn,
        AcornFlour,
        SlateRock,
        SmallSlateRock,
        LimestoneRock,
        SmallLimestoneRock,
        Flint,
        FlintShard,
        AshLog,
        BeechLog,
        PineLog,
        SpruceLog,
        AshStick,
        BeechStick,
        PineStick,
        SpruceStick,
        AshTwig,
        BeechTwig,
        PineTwig,
        SpruceTwig,
        BeechNuts,
        PineBuds,
        PinePitch,
        SprucePitch,
        PineBark,
        SpruceBark,
        SpruceCone,



    }

    public WeaponStats stats;

    

    public ItemType itemType;
    public int amount;

    public float durability;
    public float maxDurability;
   
    public string itemName()
    {
        switch (itemType)
        {
            case ItemType.Blank:
            default:
                return "Blank";
           
            case ItemType.FireAxe:
                return "Fire axe";
            case ItemType.Grenade:
                return "Grenade";
            case ItemType.IronAxe:
                return "Iron axe";
            case ItemType.FieldstoneRock:
                return "Fieldstone";
            case ItemType.SmallFieldstoneRock:
                return "Small fieldstone rock";
            case ItemType.Bone:
                return "Bone";
            case ItemType.DriedGrass:
                return "Dried grass";
            case ItemType.OakLog:
                return "Oak log";
            case ItemType.PointedRock:
                return "Pointed rock";
            case ItemType.SharpRock:
                return "Sharp rock";
            case ItemType.SmallBone:
                return "Small bone";
            case ItemType.OakStick:
                return "Oak stick";
            case ItemType.OakTwig:
                return "Oak twig";
            case ItemType.Twine:
                return "Twine";
            case ItemType.Acorn:
                return "Acorn";
            case ItemType.AcornFlour:
                return "Acorn flour";
            case ItemType.SlateRock:
                return "Slate";
            case ItemType.LimestoneRock:
                return "Limestone";
            case ItemType.SmallSlateRock:
                return "Small slate rock";
            case ItemType.SmallLimestoneRock:
                return "Small limestone rock";
            case ItemType.Flint:
                return "Flint";
            case ItemType.FlintShard:
                return "Flint shard";
            case ItemType.AshLog:
                return "Ash log";
            case ItemType.BeechLog:
                return "Beech log";
            case ItemType.PineLog:
                return "Pine log";
            case ItemType.SpruceLog:
                return "Spruce log";
            case ItemType.AshStick:
                return "Ash stick";
            case ItemType.BeechStick:
                return "Beech stick";
            case ItemType.PineStick:
                return "Pine stick";
            case ItemType.SpruceStick:
                return "Spruce stick";
            case ItemType.AshTwig:
                return "Ash twig";
            case ItemType.BeechTwig:
                return "Beech twig";
            case ItemType.PineTwig:
                return "Pine twig";
            case ItemType.SpruceTwig:
                return "Spruce twig";
            case ItemType.BeechNuts:
                return "Beech Nuts";
            case ItemType.PineBuds:
                return "Pine buds";
            case ItemType.PineBark:
                return "Pine bark";
            case ItemType.SpruceBark:
                return "Spruce bark";
            case ItemType.PinePitch:
                return "Pine Pitch";
            case ItemType.SprucePitch:
                return "Spruce Pitch";
            case ItemType.SpruceCone:
                return "Spruce cone";





        }
    }

    public string itemTooltip()
    {
        switch (itemType)
        {
            case ItemType.Blank:
            default:
                return "";
            case ItemType.Grenade:
                return "BOOM.";
            
            case ItemType.FireAxe:
                return "The emergency axe from the ship's toolbox\n Packs a strong blow.";
            case ItemType.IronAxe:
                return "A strong hatchet.";
            case ItemType.FieldstoneRock:
                return "A solid fieldstone rock.";
            case ItemType.SmallFieldstoneRock:
                return " A solid fieldstone pebble.";
            case ItemType.Bone:
                return "A large bone.";
            case ItemType.DriedGrass:
                return "Dried plant fibers. Useful for creating rope.";
            case ItemType.OakLog:
                return "A strong log made of oak.";
            case ItemType.PointedRock:
                return "A rock chipped to a blunt edge.";
            case ItemType.SharpRock:
               return "A rock chipped to a sharp point.";
            case ItemType.SmallBone:
                return "A small bone.";
            case ItemType.OakStick:
                return "A stick taken from an oak tree.";
            case ItemType.OakTwig:
                return "A small oak stick.";
            case ItemType.AshTwig:
                return "A small ash twig.";
            case ItemType.BeechTwig:
                return "A small beech twig.";
            case ItemType.PineTwig:
                return "A small piece of a pine branch.";
            case ItemType.SpruceTwig:
                return "A small twig from a spruce tree.";
            case ItemType.Twine:
                return "Rough cord made of dried plant fiber.";
            case ItemType.Acorn:
                return "A small acorn nut taken from an oak tree.";
            case ItemType.AcornFlour:
                return "A bowl of acorns crushed into a flour. Useful for baking.";
            case ItemType.SmallLimestoneRock:
                return "A small limestone rock.";
            case ItemType.LimestoneRock:
                return "A solid rock made of limestone.";
            case ItemType.SmallSlateRock:
                return "A small chunk of slate.";
            case ItemType.SlateRock:
                return "A piece of brittle slate.";
            case ItemType.Flint:
                return "A sharp piece of flint.";
            case ItemType.FlintShard:
                return "A small shard of flint.";
            case ItemType.AshLog:
                return "A large log made of ash.";
            case ItemType.BeechLog:
                return "A rough log made of beech.";
            case ItemType.PineLog:
                return "A log taken from the trunk of a pine tree.";
            case ItemType.SpruceLog:
                return "A large spruce log";
            case ItemType.AshStick:
                return "A strong stick that is very workable. Good for ranged weapons that require a lot of tension.";
            case ItemType.BeechStick:
                return "A decent stick with good flexability.";
            case ItemType.PineStick:
                return "A soft stick for makeshift handles. A good building material.";
            case ItemType.SpruceStick:
                return "A strong branch taken from a spruce tree.";
            case ItemType.PineBuds:
                return "A group of small pine buds full of sap.";
            case ItemType.BeechNuts:
                return "Small nuts with a bitter taste. Toxic unless cooked.";
            case ItemType.PineBark:
                return "A strip of sticky pine bark";
            case ItemType.SpruceBark:
                return "A strip of tough spruce bark.";
            case ItemType.PinePitch:
                return "A sticky ball of pine sap.";
            case ItemType.SprucePitch:
                return "A dark ball of spruce sap.";
            case ItemType.SpruceCone:
                return "The cone from a spruce tree. Can be used to extract pitch.";


        }
    }
    public int MaxStack()
    {
        switch (itemType)
        {
           
            
            
           
            
           

            case ItemType.IronAxe:
            case ItemType.FireAxe:
               
                return 1;
           

            case ItemType.Grenade:
            case ItemType.Twine:
            case ItemType.AcornFlour:
            case ItemType.PinePitch:
            case ItemType.SprucePitch:
                return 8;
            case ItemType.FieldstoneRock:
            case ItemType.LimestoneRock:
            case ItemType.SlateRock:
            case ItemType.DriedGrass:
            case ItemType.OakLog:
            case ItemType.PointedRock:
            case ItemType.SharpRock:
            case ItemType.Bone:
            case ItemType.OakStick:
            case ItemType.AshStick:
            case ItemType.BeechStick:
            case ItemType.PineStick:
            case ItemType.SpruceStick:
            case ItemType.Flint:
            case ItemType.AshLog:
            case ItemType.BeechLog:
            case ItemType.PineLog:
            case ItemType.SpruceLog:
            case ItemType.PineBark:
            case ItemType.SpruceBark:
            case ItemType.SpruceCone:
                return 20;
            case ItemType.SmallFieldstoneRock:
            case ItemType.SmallLimestoneRock:
            case ItemType.SmallSlateRock:
            case ItemType.SmallBone:
            case ItemType.OakTwig:
            case ItemType.AshTwig:
            case ItemType.BeechTwig:
            case ItemType.PineTwig:
            case ItemType.SpruceTwig:
            case ItemType.Acorn:
            case ItemType.FlintShard:
            case ItemType.PineBuds:
            case ItemType.BeechNuts:

                return 35;
            default:
            case ItemType.Blank:

                return 6942069;
        }
    }










    public Sprite GetSprite()
    {
        switch (itemType)
        {
            default:
            case ItemType.Blank: return ItemAssets.Instance.blankSprite;
            case ItemType.Grenade: return ItemAssets.Instance.grenadeSprite;
            case ItemType.FieldstoneRock: return ItemAssets.Instance.rockSprite;
            // backpacks

            case ItemType.FireAxe: return ItemAssets.Instance.fireaxeSprite;
            case ItemType.SmallFieldstoneRock: return ItemAssets.Instance.smallRockSprite;
            case ItemType.Acorn:return ItemAssets.Instance.acornSprite;
            case ItemType.DriedGrass: return ItemAssets.Instance.driedGrassSprite;
            case ItemType.OakLog: return ItemAssets.Instance.oakLogSprite;
            case ItemType.PointedRock: return ItemAssets.Instance.pointedRockSprite;
            case ItemType.SharpRock: return ItemAssets.Instance.sharpRockSprite;
            case ItemType.Bone: return ItemAssets.Instance.boneSprite;
            case ItemType.OakStick: return ItemAssets.Instance.oakStickSprite;
            case ItemType.Twine: return ItemAssets.Instance.twineSprite;
            case ItemType.SmallBone: return ItemAssets.Instance.smallBoneSprite;
            case ItemType.OakTwig: return ItemAssets.Instance.oakTwigSprite;
            case ItemType.AcornFlour: return ItemAssets.Instance.acornFlourSprite;
            case ItemType.SlateRock: return ItemAssets.Instance.slateSprite;
            case ItemType.SmallSlateRock:return ItemAssets.Instance.smallSlatesprite;
            case ItemType.LimestoneRock: return ItemAssets.Instance.limestoneSprite;
            case ItemType.SmallLimestoneRock: return ItemAssets.Instance.smallLimestoneSprite;
            case ItemType.Flint: return ItemAssets.Instance.flintSprite;
            case ItemType.FlintShard: return ItemAssets.Instance.flintShardSprite;
            case ItemType.AshLog: return ItemAssets.Instance.ashLogSprite;
            case ItemType.BeechLog: return ItemAssets.Instance.beechLogSprite;
            case ItemType.PineLog: return ItemAssets.Instance.pineLogSprite;
            case ItemType.SpruceLog: return ItemAssets.Instance.spruceLogSprite;
            case ItemType.AshStick:return ItemAssets.Instance.ashStickSprite;
            case ItemType.BeechStick:return ItemAssets.Instance.beechStickSprite;
            case ItemType.PineStick:return ItemAssets.Instance.pineStickSprite;
            case ItemType.SpruceStick:return ItemAssets.Instance.spruceStickSprite;
            case ItemType.AshTwig: return ItemAssets.Instance.ashTwigSprite;
            case ItemType.BeechTwig: return ItemAssets.Instance.beechTwigSprite;
            case ItemType.PineTwig: return ItemAssets.Instance.pineTwigSprite;
            case ItemType.SpruceTwig: return ItemAssets.Instance.spruceTwigSprite;
            case ItemType.PinePitch:return ItemAssets.Instance.pinePitchSprite;
            case ItemType.SprucePitch:return ItemAssets.Instance.sprucePitchSprite;
            case ItemType.PineBark:return ItemAssets.Instance.pineBarkSprite;
            case ItemType.SpruceBark:return ItemAssets.Instance.spruceBarkSprite;
            case ItemType.PineBuds:return ItemAssets.Instance.pineBudsSprite;
            case ItemType.BeechNuts:return ItemAssets.Instance.beechNutsSprite;
            case ItemType.SpruceCone: return ItemAssets.Instance.spruceConeSprite;

          


        }
    }
    // switch (itemType)
    //  {
    //   default:
    //  case ItemType.Log:
    //  case ItemType.Rock:
    //       return 3;

    //    case ItemType.Stick:
    //   case ItemType.FiberRope:
    //        return 8;

    //   case ItemType.Pistol:
    //   case ItemType.RootboundBlade:
    //      return 1;

    //    case ItemType.Blank:
    //      return 314159265;

    // return false;
    // }

    public string Slot()
    {
        switch (itemType)
        {
            case ItemType.Blank:
            default:
            case ItemType.Grenade:
            case ItemType.FireAxe:
            case ItemType.IronAxe:
            case ItemType.FieldstoneRock:
            case ItemType.SmallFieldstoneRock:
            case ItemType.DriedGrass:
            case ItemType.OakLog:
            case ItemType.PointedRock:
            case ItemType.SharpRock:
            case ItemType.Bone:
            case ItemType.OakStick:
            case ItemType.SmallBone:
            case ItemType.OakTwig:
            case ItemType.Twine:
            case ItemType.Acorn:
            case ItemType.AcornFlour:
            case ItemType.LimestoneRock:
            case ItemType.SmallLimestoneRock:
            case ItemType.SlateRock:
            case ItemType.SmallSlateRock:
            case ItemType.Flint:
            case ItemType.FlintShard:
            case ItemType.AshLog:
            case ItemType.BeechLog:
            case ItemType.PineLog:
            case ItemType.SpruceLog:
            case ItemType.AshStick:
            case ItemType.BeechStick:
            case ItemType.PineStick:
            case ItemType.SpruceStick:
            case ItemType.AshTwig:
            case ItemType.BeechTwig:
            case ItemType.PineTwig:
            case ItemType.SpruceTwig:
            case ItemType.PinePitch:
            case ItemType.SprucePitch:
            case ItemType.PineBark:
            case ItemType.SpruceBark:
            case ItemType.PineBuds:
            case ItemType.BeechNuts:
            case ItemType.SpruceCone:
                return "standard";
                /*
            case ItemType.DarkSteelHelmet:
                return "armor_head";
            case ItemType.DarkSteelChestplate:
            case ItemType.Blanket:
                return "armor_chest";
            case ItemType.DarkSteelGreaves:
                return "armor_legs";

            case ItemType.SUPREME:
                return "accessory";
            case ItemType.FiberBackpack:
            case ItemType.SciencePouches:
            case ItemType.TravelBackpack:
            case ItemType.AlienBackpack:
                return "backpack";
                */
        }



    }
    public bool IsStackable()
    {
        switch (itemType)
        {
            default:
            case ItemType.Grenade:
            case ItemType.Blank:
            case ItemType.FieldstoneRock:
            case ItemType.SmallFieldstoneRock:
            case ItemType.DriedGrass:
            case ItemType.OakLog:
            case ItemType.PointedRock:
            case ItemType.SharpRock:
            case ItemType.Bone:
            case ItemType.OakStick:
            case ItemType.SmallBone:
            case ItemType.OakTwig:
            case ItemType.Twine:
            case ItemType.Acorn:
            case ItemType.AcornFlour:
            case ItemType.LimestoneRock:
            case ItemType.SmallLimestoneRock:
            case ItemType.SlateRock:
            case ItemType.SmallSlateRock:
            case ItemType.Flint:
            case ItemType.FlintShard:
            case ItemType.AshLog:
            case ItemType.BeechLog:
            case ItemType.PineLog:
            case ItemType.SpruceLog:
            case ItemType.AshStick:
            case ItemType.BeechStick:
            case ItemType.PineStick:
            case ItemType.SpruceStick:
            case ItemType.AshTwig:
            case ItemType.BeechTwig:
            case ItemType.PineTwig:
            case ItemType.SpruceTwig:
            case ItemType.PinePitch:
            case ItemType.SprucePitch:
            case ItemType.PineBark:
            case ItemType.SpruceBark:
            case ItemType.PineBuds:
            case ItemType.BeechNuts:
            case ItemType.SpruceCone:
                return true;
           
            case ItemType.FireAxe:
            case ItemType.IronAxe:
                return false;

                // return false;
        }
    }
    public bool IsDropable()
    {
        switch (itemType)
        {
            default:
            case ItemType.Grenade:
            case ItemType.FireAxe:
            case ItemType.IronAxe:
            case ItemType.FieldstoneRock:
            case ItemType.SmallFieldstoneRock:
            case ItemType.DriedGrass:
            case ItemType.OakLog:
            case ItemType.PointedRock:
            case ItemType.SharpRock:
            case ItemType.Bone:
            case ItemType.OakStick:
            case ItemType.SmallBone:
            case ItemType.OakTwig:
            case ItemType.Twine:
            case ItemType.Acorn:
            case ItemType.AcornFlour:
            case ItemType.LimestoneRock:
            case ItemType.SmallLimestoneRock:
            case ItemType.SlateRock:
            case ItemType.SmallSlateRock:
            case ItemType.Flint:
            case ItemType.FlintShard:
            case ItemType.AshLog:
            case ItemType.BeechLog:
            case ItemType.PineLog:
            case ItemType.SpruceLog:
            case ItemType.AshStick:
            case ItemType.BeechStick:
            case ItemType.PineStick:
            case ItemType.SpruceStick:
            case ItemType.AshTwig:
            case ItemType.BeechTwig:
            case ItemType.PineTwig:
            case ItemType.SpruceTwig:
            case ItemType.PinePitch:
            case ItemType.SprucePitch:
            case ItemType.PineBark:
            case ItemType.SpruceBark:
            case ItemType.PineBuds:
            case ItemType.BeechNuts:
            case ItemType.SpruceCone:
                return true;


            case ItemType.Blank:
                 return false;
        }
    }
    public bool isUseable()
    {
        switch (itemType)
        {
            default:
            case ItemType.FieldstoneRock:
            case ItemType.SmallFieldstoneRock:
            case ItemType.DriedGrass:
            case ItemType.OakLog:
            case ItemType.PointedRock:
            case ItemType.SharpRock:
            case ItemType.Bone:
            case ItemType.OakStick:
            case ItemType.SmallBone:
            case ItemType.OakTwig:
            case ItemType.Twine:
            case ItemType.Acorn:
            case ItemType.AcornFlour:
            case ItemType.LimestoneRock:
            case ItemType.SmallLimestoneRock:
            case ItemType.SlateRock:
            case ItemType.SmallSlateRock:
            case ItemType.Flint:
            case ItemType.FlintShard:
            case ItemType.AshLog:
            case ItemType.BeechLog:
            case ItemType.PineLog:
            case ItemType.SpruceLog:
            case ItemType.AshStick:
            case ItemType.BeechStick:
            case ItemType.PineStick:
            case ItemType.SpruceStick:
            case ItemType.AshTwig:
            case ItemType.BeechTwig:
            case ItemType.PineTwig:
            case ItemType.SpruceTwig:
            case ItemType.PinePitch:
            case ItemType.SprucePitch:
            case ItemType.PineBark:
            case ItemType.SpruceBark:
            case ItemType.PineBuds:
            case ItemType.BeechNuts:
            case ItemType.SpruceCone:
                return false;
           
            
            case ItemType.Grenade:
              
                return true;
          

        }
    }
    //allows per slot backpack level
    public float Level()
    {
        switch (itemType)
        {
            default:
            case ItemType.Grenade:
            case ItemType.FireAxe:
            case ItemType.Blank:
            case ItemType.IronAxe:
            case ItemType.FieldstoneRock:
            case ItemType.SmallFieldstoneRock:
            case ItemType.DriedGrass:
            case ItemType.OakLog:
            case ItemType.PointedRock:
            case ItemType.SharpRock:
            case ItemType.Bone:
            case ItemType.OakStick:
            case ItemType.SmallBone:
            case ItemType.OakTwig:
            case ItemType.Twine:
            case ItemType.Acorn:
            case ItemType.AcornFlour:
            case ItemType.LimestoneRock:
            case ItemType.SmallLimestoneRock:
            case ItemType.SlateRock:
            case ItemType.SmallSlateRock:
            case ItemType.Flint:
            case ItemType.FlintShard:
            case ItemType.AshLog:
            case ItemType.BeechLog:
            case ItemType.PineLog:
            case ItemType.SpruceLog:
            case ItemType.AshStick:
            case ItemType.BeechStick:
            case ItemType.PineStick:
            case ItemType.SpruceStick:
            case ItemType.AshTwig:
            case ItemType.BeechTwig:
            case ItemType.PineTwig:
            case ItemType.SpruceTwig:
            case ItemType.PinePitch:
            case ItemType.SprucePitch:
            case ItemType.PineBark:
            case ItemType.SpruceBark:
            case ItemType.PineBuds:
            case ItemType.BeechNuts:
            case ItemType.SpruceCone:
                return 0f;
            
        }
    }
   
    public bool HasDur()
    {
        switch (itemType)
        {
            case ItemType.IronAxe:
            case ItemType.FireAxe:
                return true;
            default:
            case ItemType.Grenade:
            case ItemType.Blank:
            case ItemType.FieldstoneRock:
            case ItemType.SmallFieldstoneRock:
            case ItemType.DriedGrass:
            case ItemType.OakLog:
            case ItemType.PointedRock:
            case ItemType.SharpRock:
            case ItemType.Bone:
            case ItemType.OakStick:
            case ItemType.SmallBone:
            case ItemType.OakTwig:
            case ItemType.Twine:
            case ItemType.Acorn:
            case ItemType.AcornFlour:
            case ItemType.LimestoneRock:
            case ItemType.SmallLimestoneRock:
            case ItemType.SlateRock:
            case ItemType.SmallSlateRock:
            case ItemType.Flint:
            case ItemType.FlintShard:
            case ItemType.AshLog:
            case ItemType.BeechLog:
            case ItemType.PineLog:
            case ItemType.SpruceLog:
            case ItemType.AshStick:
            case ItemType.BeechStick:
            case ItemType.PineStick:
            case ItemType.SpruceStick:
            case ItemType.AshTwig:
            case ItemType.BeechTwig:
            case ItemType.PineTwig:
            case ItemType.SpruceTwig:
            case ItemType.PinePitch:
            case ItemType.SprucePitch:
            case ItemType.PineBark:
            case ItemType.SpruceBark:
            case ItemType.PineBuds:
            case ItemType.BeechNuts:
            case ItemType.SpruceCone:
                return false;
        }
    }


}
[Serializable]
public class droppedItem
{
    public GameObject itemPrefab;
    public int minAmnt;
    public int maxAmnt;
    public int objMinAmnt;
    public int objMaxAmnt;
};

