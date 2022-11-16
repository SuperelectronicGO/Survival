using System.Collections;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RiverMeshGenerator))]

public class RiverEditor : Editor
{
	public override void OnInspectorGUI()
	{
		RiverMeshGenerator mapGen = (RiverMeshGenerator)target;

		if (DrawDefaultInspector())
		{

		}

		if (GUILayout.Button("Generate"))
		{
			mapGen.generateMesh(true);
		}
	}
}
