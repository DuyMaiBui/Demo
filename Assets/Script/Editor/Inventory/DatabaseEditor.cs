using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemDatabaseSO))]
public class ItemEditor : Editor
{
    ItemDatabaseSO data;

    private void OnEnable()
    {
        data = (ItemDatabaseSO)target;
    }

    public override void OnInspectorGUI()
    {
        this.DrawDefaultInspector();
        if (GUILayout.Button("Update Item ID"))
        {
            data.UpdateItemID();
        }
        if (GUILayout.Button("Clear Item"))
        {
            data.ClearItem();
        }

        EditorUtility.SetDirty(data);
    }
}
