using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Consume Item", menuName ="Inventory System/Item/Consume")]
public class ConsumeSO : ItemSO
{
    public BuffItemStat[] m_Stats;

    public override BaseSO GetBase() { return null; }

    public override ConsumeSO GetConsume() { return this; }

    public override EquipmentSO GetEquipment() { return null; }
}