using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTags : MonoBehaviour
{
    public enum TagType
    {
        Player,
        Worm,
    }
    public TagType[] tags;
}
