using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Collections;
using Random = UnityEngine.Random;
public class RandomLayoutCreator : MonoBehaviour
{
    [SerializeField] private bool spawnOnStart;
    [NonReorderable]
    public objectPrefab[] prefabs;
    [Serializable]
    public struct objectPrefab
    {
        public string name;
        public GameObject prefab;
        public ushort id;
    }
    public string[] layouts = { "0,(4.37, 0.00, -2.91),(0.00000, 0.38268, 0.00000, 0.92388),(0.80, 0.80, 0.80)!0,(0.00, 0.00, 0.00),(0.00000, 0.00000, 0.00000, 1.00000),(1.00, 1.00, 1.00)!0,(4.65, -0.02, -0.07),(0.69159, -0.14731, 0.14731, 0.69159),(0.40, 0.40, 0.40)!1,(2.44, -1.09, 1.62),(-0.70711, 0.00000, 0.00000, 0.70711),(1.00, 1.00, 1.00)!" };

    private void Start()
    {
        if (spawnOnStart) CreateLayout();
    }

    //Method that creates a layout
    public void CreateLayout()
    {
        //Choose a random layout
        int randomLayout = Random.Range(0, layouts.Length);
        //Get the string array with all the objects
        string[] objs = DeconstructString(layouts[randomLayout], "!");
        //Loop through for each object, disregard the last element because it will be blank
        for(int i = 0; i < objs.Length - 1; i++)
        {
            //Split the object string into its values and assign those 
            string[] values = DeconstructString(objs[i], "@");
            Debug.Log(values[0]);
            ushort objectId = ushort.Parse(values[0]);
            Vector3 pos = ToVector3(values[1]);
            Quaternion rot = ToQuaternion(values[2]);
            Vector3 scl = ToVector3(values[3]);
            GameObject spawnObject = ObjectFromId(objectId);
            //Divide by scale to account for wonky parent scaling 
            pos = new Vector3(pos.x / scl.x, pos.y / scl.y, pos.z / scl.z) + transform.position;
;           GameObject g = Instantiate(spawnObject, pos, rot, this.transform);
            g.transform.localScale = scl;
        }
    }
    //Returns an array of the string split by splitter
    private string[] DeconstructString(string str, string splitter)
    {
        return str.Split(splitter);
    }
    //Returns the Vector3 created from a string
    private Vector3 ToVector3(string str)
    {
        string valString = str;
        valString = valString.Substring(1, valString.Length - 2);
        Debug.Log(valString);
        string[] subs = valString.Split(",");
        Vector3 result = new Vector3(
            float.Parse(subs[0]),
            float.Parse(subs[1]),
            float.Parse(subs[2]));
        return result;
    }
    //Returns the Quaternion created from a string
    private Quaternion ToQuaternion(string str)
    {
        string valString = str;
        valString = valString.Substring(1, valString.Length - 2);
        string[] subs = valString.Split(",");
        Quaternion result = new Quaternion(
            float.Parse(subs[0]),
            float.Parse(subs[1]),
            float.Parse(subs[2]),
            float.Parse(subs[3]));
        return result;
    }
    //Finds the GameObject with the given id
    private GameObject ObjectFromId(ushort id)
    {
        for(int i=0; i<prefabs.Length; i++)
        {
            if(prefabs[i].id == id)
            {
                return prefabs[i].prefab;
            }
        }
        throw new NotImplementedException($"No object with id {id} was found in the prefab list");
    }
    //Method that copys the current layout as a string to the clipboard
    public void CopyLayoutString()
    {
        

        string copiedString = "";
        for(int i=0; i<transform.childCount; i++)
        {
            Transform chi = transform.GetChild(i);
            //Check if prefab is registered
            ushort objectId = GetPrefabId(chi);
            string addString = $"{objectId}@{chi.localPosition}@{chi.localRotation}@{chi.localScale}!";
            copiedString += addString;
        }
        GUIUtility.systemCopyBuffer = copiedString;
    }
    //Finds the id from the given transform
    private ushort GetPrefabId(Transform t)
    {
        foreach (objectPrefab p in prefabs)
        {
            if (p.name == t.name)
            {
                return p.id;

            }
        }
        throw new NotImplementedException($"Object {t.name} isn't registed in the prefab list. Check if the names match exactly.");
        
    }
}
