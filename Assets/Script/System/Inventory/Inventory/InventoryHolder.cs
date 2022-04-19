using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InventoryHolder : ItemHolder
{
    private int slot1_ID;
    private int slot2_ID;
    private int slot1_Amount;
    private int slot2_Amount;
    private float slot1_Endurance;
    private float slot2_Endurance;

    private int addAmount;

    private bool canCraft;

    private void OnEnable()
    {
        EventDispatcher.CraftEvent += Craft;
        EventDispatcher.CheckRecipeEvent += CheckRecipe;
        EventDispatcher.AddItemEvent += AddItem;
    }

    private void OnDisable()
    {
        EventDispatcher.CraftEvent -= Craft;
        EventDispatcher.CheckRecipeEvent -= CheckRecipe;
        EventDispatcher.AddItemEvent -= AddItem;
    }
    private void AddItem(ItemHolderType type, ItemGO item)
    {
        if (type == this.type)
        {
            AddItem(item.data.item.ID, item.data.amount, item.data.endurance);
            EventDispatcher.DecreaseAmount(item, addAmount);
        }
    }

    private void AddItem(int id, int amount, float endurance)
    {
        if (!CheckCanAddItem(id, amount, endurance))
        {
            var go = ObjectPoolManager.instance.InstanceGameObject(inventorySO.database.GetItemWithID(id).itemName);
            go.GetComponent<ItemGO>().SetAmount(amount - addAmount);
            EventDispatcher.SetItemParent(go);
        }
        EventDispatcher.SetFloatText(string.Concat(" + ", addAmount, " ", inventorySO.database.GetItemWithID(id).name));
    }

    private bool CheckCanAddItem(int id, int amount, float endurance)
    {
        addAmount = 0;
        tempItem = inventorySO.database.GetItemWithID(id);
        while (amount > 0)
        {
            tempSlot = GetSlotWithID(id);
            if (tempSlot != null)
            {
                int remainAmount = tempItem.maxAmount - tempSlot.data.m_Amount;
                if (amount > remainAmount)
                {
                    tempSlot.UpdateUI(id, tempItem.maxAmount, endurance);
                    addAmount += remainAmount;
                    amount -= remainAmount;
                }
                else
                {
                    tempSlot.UpdateUI(id, tempSlot.data.m_Amount + amount, endurance);
                    addAmount += amount;
                    amount = 0;
                }
            }
            else
            {
                tempSlot = GetSlotEmpty;
                if (tempSlot != null)
                {
                    int remainAmount = tempItem.maxAmount - tempSlot.data.m_Amount;
                    if (amount > remainAmount)
                    {
                        tempSlot.UpdateUI(id, tempItem.maxAmount, endurance);
                        addAmount += remainAmount;
                        amount -= remainAmount;
                    }
                    else
                    {
                        tempSlot.UpdateUI(id, tempSlot.data.m_Amount + amount, endurance);
                        addAmount += amount;
                        amount = 0;
                    };
                }
                else
                {
                    amount = 0;
                    return false;
                }
            }
        }
        return true;
    }

    // Craft
    private void CheckRecipe(RecipeSO recipe, Button button)
    {
        if (recipe == null)
            return;
        int counter = 0;
        for (int i = 0; i < recipe.ingredientItem.Length; i++)
        {
            if (GetAmountItem(recipe.ingredientItem[i].ingredientItem.ID) >= recipe.ingredientItem[i].amount)
                counter++;
        }
        if (counter == recipe.ingredientItem.Length)
        {
            button.interactable = true;
            canCraft = true;
        }
        else
        {
            button.interactable = false;
            canCraft = false;
        }
    }

    private void Craft(RecipeSO recipe, Image fill)
    {
        if (canCraft)
            StartCoroutine(WaitCraft(recipe, fill));
    }

    IEnumerator WaitCraft(RecipeSO recipe, Image fill)
    {
        float lerp = 0;
        lerp = Mathf.MoveTowards(lerp, 1, recipe.timeCraft);
        fill.fillAmount = Mathf.Lerp(0, 1, lerp);
        yield return new WaitForSecondsRealtime(recipe.timeCraft);
        for (int i = 0; i < recipe.ingredientItem.Length; i++)
        {
            RemoveItem(recipe.ingredientItem[i].ingredientItem.ID, recipe.ingredientItem[i].amount);
        }
        AddItem(recipe.resultItem.ID, recipe.resultItem.amountToAdd, recipe.resultItem.enduranceAmount);
        CheckRecipe(recipe, fill.transform.parent.GetComponent<Button>());
        fill.fillAmount = 0;
    }

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
                if (slot1.data.m_ID == slot2.data.m_ID)
                {
                    var tempItem = inventorySO.database.GetItemWithID(slot2.data.m_ID);
                    var tempAmount = tempItem.maxAmount - slot2.data.m_Amount;
                    if (slot1.data.m_Amount <= tempAmount)
                    {
                        slot2.UpdateUI(slot2_ID, slot2_Amount + tempAmount, slot2_Endurance);
                        slot1.UpdateUI(-1, 0, 0);
                        return;
                    }
                    slot2.UpdateUI(slot2_ID, slot2_Amount + tempAmount, slot2_Endurance);
                    slot1.UpdateUI(slot1_ID, slot1_Amount - tempAmount, slot1_Endurance);
                    return;
                }
                slot2.UpdateUI(slot1_ID, slot1_Amount, slot1_Endurance);
                slot1.UpdateUI(slot2_ID, slot2_Amount, slot2_Endurance);
                break;
            case SlotCatalog.Equipment:
                if (slot2.CheckItem(slot1_ID))
                {
                    slot2.UpdateUI(slot1_ID, slot1_Amount, slot1_Endurance);
                    slot1.UpdateUI(slot2_ID, slot2_Amount, slot2_Endurance);
                }
                break;
            default:
                break;
        }
    }

    protected override void RightClickSlot(Slot slot)
    {
        ItemSO itemInSlot = inventorySO.database.GetItemWithID(slot.data.m_ID);
        if (itemInSlot.GetBase() != null)
            return;
        var buff = itemInSlot.GetConsume();
        if (buff != null)
        {
            for (int i = 0; i < buff.m_Stats.Length; i++)
            {
                EventDispatcher.ModifierStat(buff.m_Stats[i].stat, buff.m_Stats[i].value);
                EventDispatcher.SetFloatText(string.Concat(" + ", buff.m_Stats[i].value, " ", buff.m_Stats[i].stat.ToString()));
            }
            slot.UpdateUI(slot.data.m_ID, slot.data.m_Amount - 1, slot.data.m_Endurance);
            return;
        }
        var equip = itemInSlot.GetEquipment();
        if (equip != null)
        {
            switch (equip.catalog)
            {
                case EquipmentCatalog.None:
                    break;
                case EquipmentCatalog.Armor:
                    Swap(slot, extraHolder.slots[2]);
                    break;
                case EquipmentCatalog.Hat:
                    Swap(slot, extraHolder.slots[1]);
                    break;
                case EquipmentCatalog.Weapon:
                    Swap(slot, extraHolder.slots[0]);
                    break;
                default:
                    break;
            }
        }
    }

    // Tool Tip
    protected override void ShowToolTip(ItemSO item)
    {
        if (item.GetBase() != null)
            EventDispatcher.ShowToolTip(_input.position, item.itemName);
        else
            EventDispatcher.ShowToolTip(_input.position, string.Concat(item.itemName, "\n[RMB] Use"));
    }
}
