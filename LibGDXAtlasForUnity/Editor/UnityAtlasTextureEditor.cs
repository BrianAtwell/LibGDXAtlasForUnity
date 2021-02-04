using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AtlasTextureImporter
{
    [CustomEditor(typeof(UnityAtlasTextureImporter))]
    public class UnityAtlasTextureImporterEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            var colorShift = new GUIContent("Color Shift");
            var prop = serializedObject.FindProperty("m_ColorShift");
            EditorGUILayout.PropertyField(prop, colorShift);
            base.ApplyRevertGUI();
        }
    }
}