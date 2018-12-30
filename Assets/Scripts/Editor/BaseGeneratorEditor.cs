using UnityEditor;
using UnityEngine;

namespace EditorScripts
{
	/// <summary>
	/// Renders additional UI Elements to all BaseGenerator Unity-Inspectors
	/// </summary>
	[CustomEditor(typeof(BaseGenerator), true)]
	public class BaseGeneratorEditor : Editor
	{
		#region Public methods

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			BaseGenerator gen = (BaseGenerator) target;

			if (GUILayout.Button("Generate"))
			{
				gen.Generate();
			}

			if (GUILayout.Button("Clear"))
			{
				gen.Clear();
			}
		}

		#endregion
	}
}