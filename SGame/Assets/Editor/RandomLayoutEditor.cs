using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomLayoutCreator))]
public class RandomLayoutEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RandomLayoutCreator layoutCreator = (RandomLayoutCreator)target;
        if (DrawDefaultInspector())
        {

        }
        if(GUILayout.Button("Copy Values"))
        {
            layoutCreator.CopyLayoutString();
        }
        if (GUILayout.Button("Create Layout"))
        {
            layoutCreator.CreateLayout();
        }
    }
}
