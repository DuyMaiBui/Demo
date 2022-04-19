using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentHolder : ItemHolder
{
    private int slot1_ID;
    private int slot2_ID;
    private int slot1_Amount;
    private int slot2_Amount;
    private float slot1_Endurance;
    private float slot2_Endurance;

    // Swap
    protected override void Swap(Slot slot1, Slot slot2)
    {
        if (slot1 == null)
            return;
        if (slot2 == null)
            return;
        if (slot1 == slot2)
            return;

        slot1_ID = slot1.data.m_ID;
        slot2_ID = slot2.data.m_ID;
        slot1_Amount = slot1.data.m_Amount;
        slot2_Amount = slot2.data.m_Amount;
        slot1_Endurance = slot1.data.m_Endurance;
        slot2_Endurance = slot2.data.m_Endurance;

        switch (slot2.slotCatalog)
        {
            case SlotCatalog.None:
                break;
            case SlotCatalog.Inventory:
                if (slot1.CheckItem(slot2_ID))
                {
                    slot2.UpdateUI(slot1_ID, slot1_Amount, slot1_Endurance);
                    slot1.UpdateUI(slot2_ID, slot2_Amount, slot2_Endurance);
                }
                break;
            case SlotCatalog.Equipment:
                break;
            default:
                break;
        }
    }

    protected override void RightClickSlot(Slot slot)
    {
        var emptySlot = extraHolder.GetSlotEmpty;
        if (emptySlot == null)
            return;
        emptySlot.UpdateUI(slot.data.m_ID, slot.data.m_Amount, slot.data.m_Endurance);
        slot.UpdateUI(-1, 0, 0);
    }

    // Tool Tip
    protected override void ShowToolTip(ItemSO item)
    {
        EventDispatcher.ShowToolTip(_input.position, string.Concat(item.itemName, "\n[RMB] UnEquip"));
    }
}
