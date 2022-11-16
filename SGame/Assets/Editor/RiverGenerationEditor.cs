using System.Collections;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RiverGeneration))]

public class RiverGenerationEditor : Editor
{
	public override void OnInspectorGUI()
	{
		RiverGeneration mapGen = (RiverGeneration)target;

		if (DrawDefaultInspector())
		{

		}

		if (GUILayout.Button("Generate"))
		{
			mapGen.GenerateRiverComponents();
		}
		if (GUILayout.Button("Generate Terrain Heightmap"))
		{
			mapGen.GenerateCompleteTerrainHeightmap();
		}


	}
}
