using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGO : MonoBehaviour
{
    public ItemSaveData data;

    private void OnEnable()
    {
        EventDispatcher.DecreaseAmountEvent += DecreaseAmountItem;
        EventDispatcher.SaveItemDataEvent += SaveItem;
        SetAmount(data.item.amountToAdd);
    }

    private void OnDisable()
    {
        EventDispatcher.DecreaseAmountEvent -= DecreaseAmountItem;
        EventDispatcher.SaveItemDataEvent -= SaveItem;
    }

    public void SaveItem(List<ItemSaveData> list)
    {
        data.UpdateData(transform.position, transform.rotation, data.item, data.amount, data.endurance);
        list.Add(data);
    }

    public void SetAmount(int amount)
    {
        this.data.amount = amount;
    }

    private void DecreaseAmountItem(ItemGO item, int amount)
    {
        if (item != this)
            return;
        UpdateItemGO(amount);
    }

    private void UpdateItemGO(int amount)
    {
        data.amount -= amount;
        if (data.amount <= 0)
        {
            ObjectPoolManager.instance.DeactiveGameObject(data.item.itemName, gameObject);
        }
    }
}

[System.Serializable]
public struct ItemSaveData
{
    public Vector3 position;
    public Quaternion rotation;
    public ItemSO item;
    public int amount;
    public float endurance;

    public void UpdateData(Vector3 position, Quaternion rotation, ItemSO item, int amount, float endurance)
    {
        this.position = position;
        this.rotation = rotation;
        this.item = item;
        this.amount = amount;
        this.endurance = endurance;
    }
}
