using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemSO : ScriptableObject
{
    [Header("Item")]
    public string itemName;
    public GameObject perfab;
    public Sprite iconItem;
    public string description;

    [Header("Endurance")]
    public bool hasEndurance;
    public float enduranceAmount = 100f;
    public float enduranceRatio = 0f;

    [Header("Data")]
    public int ID;
    public int maxAmount;
    public int amountToAdd;

    public abstract BaseSO GetBase();
    public abstract EquipmentSO GetEquipment();
    public abstract ConsumeSO GetConsume();
}
