using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot : Slot
{
    public EquipmentCatalog catalog;
    public EquipmentSO newEquipment;
    public EquipmentSO lastEquipment;

    public override void UpdateEndurance(float enduranceChange)
    {
        data.m_Endurance -= enduranceChange;
        if (data.m_Endurance <= 0)
        {
            if (slotIndex == 0)
                EventDispatcher.ChangeEquipment(newEquipment, null);
            ResetSlot();
        }
        else
        {
            var item = inventory.database.GetItemWithID(data.m_ID);
            isFull = true;
            if (item != null)
                enduranceFill.fillAmount = data.m_Endurance / 100f;
        }
        inventory.UpdateData(slotIndex, data.m_ID, data.m_Amount, data.m_Endurance);
    }

    public override void UpdateUI(int id, int amount, float endurance)
    {
        if (!CheckItem(id))
            return;
        if (id == -1 || amount == 0)
        {
            ResetSlot();
        }
        else if (id != -1)
        {
            var item = inventory.database.GetItemWithID(id);
            if (item.hasEndurance)
            {
                if (endurance == 0)
                    ResetSlot();
                else
                {
                    enduranceFill.fillAmount = endurance / 100f;
                    SetDisplay(item, amount, endurance);
                }
            }
            else
            {
                enduranceFill.fillAmount = 0f;
                SetDisplay(item, amount, endurance);
            }
        }
        if (slotIndex == 0)
            EventDispatcher.ChangeEquipment(lastEquipment, newEquipment);
        inventory.UpdateData(slotIndex, data.m_ID, data.m_Amount, data.m_Endurance);
    }

    public override bool CheckItem(int id)
    {
        if (id == -1)
            return true;
        var item = inventory.database.GetItemWithID(id).GetEquipment();
        if (item == null)
            return false;
        if (item.catalog == catalog)
            return true;
        return false;
    }

    private void ResetSlot()
    {
        itemIcon.enabled = false;
        lastEquipment = newEquipment;
        newEquipment = null;
        isFull = false;
        enduranceFill.fillAmount = 0;
        data.UpdateSlotData(-1, 0, 0);
    }

    private void SetDisplay(ItemSO item, int amount, float endurance)
    {
        lastEquipment = newEquipment;
        newEquipment = item.GetEquipment();
        itemIcon.enabled = true;
        itemIcon.sprite = item.iconItem;
        isFull = true;
        data.UpdateSlotData(item.ID, amount, endurance);
    }
}
