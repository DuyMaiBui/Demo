using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectPoolManager))]
public class ObjectPoolEditor : Editor
{
    ObjectPoolManager manager;

    private void OnEnable()
    {
        manager = (ObjectPoolManager)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Set Up Dictionary", GUILayout.MaxHeight(50)))
        {
            manager.poolDictionary.Clear();
            manager.SetDictionary();
        }
        if (GUILayout.Button("Clear Dictionary", GUILayout.MaxHeight(50)))
            manager.poolDictionary.Clear();
        GUILayout.EndHorizontal();

        EditorUtility.SetDirty(manager);
    }
}
