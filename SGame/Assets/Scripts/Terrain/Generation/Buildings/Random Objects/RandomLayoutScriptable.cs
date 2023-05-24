using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Layout Scriptable", menuName = "Scriptables/Layout Scriptable", order = 9)]
public class RandomLayoutScriptable : ScriptableObject
{
    [NonReorderable]
    public LayoutGroup[] layoutGroups;
}
[System.Serializable]
public class LayoutGroup
{
    public string name;
    [NonReorderable]
    public objectPrefab[] prefabs;
    [System.Serializable]
    public struct objectPrefab
    {
        public string name;
        public GameObject prefab;
        public ushort id;
    }
    [NonReorderable]
    public string[] layoutStrings = { "0,(4.37, 0.00, -2.91),(0.00000, 0.38268, 0.00000, 0.92388),(0.80, 0.80, 0.80)!0,(0.00, 0.00, 0.00),(0.00000, 0.00000, 0.00000, 1.00000),(1.00, 1.00, 1.00)!0,(4.65, -0.02, -0.07),(0.69159, -0.14731, 0.14731, 0.69159),(0.40, 0.40, 0.40)!1,(2.44, -1.09, 1.62),(-0.70711, 0.00000, 0.00000, 0.70711),(1.00, 1.00, 1.00)!" };
}
