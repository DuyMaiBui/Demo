using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraFollow))]
public class CameraFollowEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CameraFollow cam = (CameraFollow)target;
        if (DrawDefaultInspector())
            cam.Follow();
    }
}
