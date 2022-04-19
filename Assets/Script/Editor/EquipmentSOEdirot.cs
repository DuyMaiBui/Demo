using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EquipmentSO))]
public class EquipmentSOEdirot : Editor
{
    EquipmentSO equipment;
    private void OnEnable()
    {
        equipment = (EquipmentSO)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if(equipment.catalog == EquipmentCatalog.Weapon)
        {
            
        }
    }
}
