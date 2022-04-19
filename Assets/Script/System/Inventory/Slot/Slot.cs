using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Slot : MonoBehaviour
{
    public SlotCatalog slotCatalog;
    public InventorySO inventory;
    public Image itemIcon;
    public Image enduranceFill;
    public int slotIndex;
    public bool isFull;
    public SlotData data;

    private void OnEnable()
    {
        itemIcon.enabled = false;
    }

    public abstract void UpdateEndurance(float endurance);

    public abstract void UpdateUI(int id, int amount, float enduranceChange);
    public abstract bool CheckItem(int id);
}
public enum SlotCatalog
{
    None,
    Inventory,
    Equipment
}