using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventorySlot : Slot
{
    public TextMeshProUGUI amountText;

    public override void UpdateEndurance(float enduranceChange)
    {
        data.m_Endurance -= enduranceChange;
        if (data.m_Endurance <= 0)
            ResetSlot();
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
                enduranceFill.fillAmount = 0;
                SetDisplay(item, amount, endurance);
            }
        }
        inventory.UpdateData(slotIndex, data.m_ID, data.m_Amount, data.m_Endurance);
    }
    public override bool CheckItem(int id)
    {
        return true;
    }

    private void ResetSlot()
    {
        itemIcon.enabled = false;
        amountText.text = string.Empty;
        enduranceFill.fillAmount = 0;
        isFull = false;
        data.UpdateSlotData(-1, 0, 0);
    }

    private void SetDisplay(ItemSO item, int amount, float endurance)
    {
        itemIcon.enabled = true;
        itemIcon.sprite = item.iconItem;
        amountText.text = amount > 1 ? amount.ToString() : string.Empty;
        isFull = amount < item.maxAmount ? false : true;
        data.UpdateSlotData(item.ID, amount, endurance);
    }
}
