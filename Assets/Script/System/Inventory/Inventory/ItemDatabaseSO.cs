using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Database", menuName ="Inventory System/Database")]
public class ItemDatabaseSO : ScriptableObject
{
    public List<ItemSO> items;

    public ItemSO GetItemWithID(int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (id == i)
                return items[i];
        }
        return null;
    }

    [ContextMenu("Update ID")]
    public void UpdateItemID()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].ID = i;
        }
    }

    [ContextMenu("Clear")]
    public void ClearItem()
    {
        items.Clear();
    }
}
