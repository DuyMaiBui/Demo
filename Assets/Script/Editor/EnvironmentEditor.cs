using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SaveDataParent))]
public class EnvironmentEditor : Editor
{
    SaveDataParent data;

    private void OnEnable()
    {
        data = (SaveDataParent)target;
    }

    public override void OnInspectorGUI()
    {
        this.DrawDefaultInspector();
        if(GUILayout.Button("Load Environment"))
        {
            for (int i = 0; i < data.saveDataChilds.Length; i++)
            {
                data.LoadEnvironment();
            }
        }
    }
}
