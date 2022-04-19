using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    MapGenerator mapGen;

    public void OnEnable()
    {
        mapGen = (MapGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        this.DrawDefaultInspector();
        GUILayout.Label("Generate Map");
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Generate Map", GUILayout.MaxHeight(50)))
        {
            mapGen.GenMap();
        }
        if(GUILayout.Button("Delete Map", GUILayout.MaxHeight(50)))
        {
            mapGen.RemoveMap();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Deactive Map", GUILayout.MaxHeight(50)))
        {
            mapGen.DeactiveMap();
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("Save Load");
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Save", GUILayout.MaxHeight(50)))
        {
            mapGen.SaveMap();
        }
        if (GUILayout.Button("Load", GUILayout.MaxHeight(50)))
        {
            mapGen.LoadMap();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Clear", GUILayout.MaxHeight(50)))
        {
            mapGen.ClearSaveData();
        }
    }
}