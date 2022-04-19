using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Equipment Item", menuName ="Inventory System/Item/Equipment")]
public class EquipmentSO : ItemSO
{
    public float rangeHit;
    public EquipmentCatalog catalog;
    public LayerMask layerInteract;
    public BuffItemStat[] stats;
    public override BaseSO GetBase() { return null; }

    public override ConsumeSO GetConsume() { return null; }

    public override EquipmentSO GetEquipment() { return this; }
}

[System.Serializable]
public struct BuffItemStat
{
    public BuffStat stat;
    public float value;

    public void UpdateStat(float value)
    {
        this.value = value;
    }
}

public enum BuffStat
{
    None,
    Health,
    Food,
    Water,
    Armor,
    Strength,
    Speed,
}
public enum EquipmentCatalog
{
    None,
    Armor,
    Hat,
    Weapon
}