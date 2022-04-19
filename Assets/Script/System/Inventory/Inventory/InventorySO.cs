using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Inventory System/Inventory")]
public class InventorySO : ScriptableObject
{
    public string saveKey;
    public ItemDatabaseSO database;
    public SlotData[] slotDatas;

    private string saveJson;

    public void UpdateData(int index, int id, int amount, float endurance)
    {
        slotDatas[index].UpdateSlotData(id, amount, endurance);
    }

    public void Reset()
    {
        for (int i = 0; i < slotDatas.Length; i++)
        {
            slotDatas[i].UpdateSlotData(-1, 0, 0);
        }
    }

    public void Save()
    {
        saveJson = JsonHelper.ToJon(slotDatas);
        PlayerPrefs.SetString(saveKey, saveJson);
        Debug.Log(saveJson);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        if (!PlayerPrefs.HasKey(saveKey))
            return;
        saveJson = PlayerPrefs.GetString(saveKey, saveJson);
        slotDatas = JsonHelper.FromJson<SlotData>(saveJson);
    }


}
[System.Serializable]
public class SlotData
{
    public int m_ID;
    public int m_Amount;
    public float m_Endurance;

    public SlotData()
    {
        m_ID = -1;
        m_Amount = 0;
        m_Endurance = 0;
    }

    public void UpdateSlotData(int id, int amount, float endrurance)
    {
        m_ID = id;
        m_Amount = amount;
        m_Endurance = endrurance;
    }
}