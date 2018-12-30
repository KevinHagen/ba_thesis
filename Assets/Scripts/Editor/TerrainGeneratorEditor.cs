using System.Collections;
using System.Collections.Generic;
using EditorScripts;
using PerlinNoise;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : BaseGeneratorEditor {
	public override void OnInspectorGUI()
	{
		TerrainGenerator gen = (TerrainGenerator)target;

		if (DrawDefaultInspector() && gen.AutoUpdate)
			gen.Generate();

		if (GUILayout.Button("Generate"))
		{
			gen.Generate();
		}

		if (GUILayout.Button("Clear"))
		{
			gen.Clear();
		}
	}
}
