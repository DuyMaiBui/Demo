using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RadialLayout))]
public class RadialLayoutEditor : Editor
{
    RadialLayout layout;

    private void OnEnable()
    {
        layout = (RadialLayout)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        layout.UpdateEditor();
    }
}
