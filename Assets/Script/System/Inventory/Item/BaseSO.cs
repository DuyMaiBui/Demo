using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Inventory System/Item/Base")]
public class BaseSO : ItemSO
{
    public override BaseSO GetBase() { return this; }

    public override ConsumeSO GetConsume() { return null; }

    public override EquipmentSO GetEquipment() { return null; }
}
