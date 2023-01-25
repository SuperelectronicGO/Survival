using System.Collections;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NoisemapDisplay))]

public class MapDisplayEditor : Editor
{
	public override void OnInspectorGUI()
	{
		NoisemapDisplay noiseDisplay = (NoisemapDisplay)target;

		if (DrawDefaultInspector())
		{

		}

		if (GUILayout.Button("Generate"))
		{
			noiseDisplay.displayMap();
		}
	}
}
