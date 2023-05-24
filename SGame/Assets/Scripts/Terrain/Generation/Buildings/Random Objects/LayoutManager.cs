using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutManager : MonoBehaviour
{
    [SerializeField] private RandomLayoutCreator[] layouts;
    public int GetLayoutArrayLength()
    {
        return layouts.Length;
    }
}
