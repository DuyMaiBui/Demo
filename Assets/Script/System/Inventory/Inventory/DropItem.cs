using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class DropItem : MonoBehaviour
{
    [SerializeField] private ItemDatabaseSO database;
    [SerializeField] private Image binImage;
    [SerializeField] private Sprite binOpen;
    [SerializeField] private Sprite binClose;

    private bool hoverBin;

    private void Awake()
    {
        AddEventTrigger.AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { BinState(true); hoverBin = true; });
        AddEventTrigger.AddEvent(gameObject, EventTriggerType.PointerExit, delegate { BinState(false); hoverBin = false; });
        EnableState(false);
    }

    private void OnEnable()
    {
        EventDispatcher.DropItemEvent += Drop;
        EventDispatcher.EnableStateBinEvent += EnableState;
    }

    private void OnDisable()
    {
        EventDispatcher.DropItemEvent -= Drop;
        EventDispatcher.EnableStateBinEvent -= EnableState;
    }

    private void EnableState(bool state)
    {
        if (state)
            transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.Flash);
        else
            transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.Flash);
    }

    private void BinState(bool state)
    {
        if (state)
        {
            binImage.sprite = binOpen;
        }
        else
        {
            binImage.sprite = binClose;
        }
    }

    private void Drop(Slot slot, int amount, Vector3 position)
    {
        if (!hoverBin)
            return;
        var itemDrop = ObjectPoolManager.instance.InstanceGameObject(database.GetItemWithID(slot.data.m_ID).perfab.name);
        itemDrop.transform.position = position;
        EventDispatcher.SetItemParent(itemDrop);
        var data = itemDrop.GetComponent<ItemGO>();
        data.SetAmount(amount);
        data.data.endurance = slot.data.m_Endurance;

        slot.UpdateUI(-1, 0, 0f);
    }
}
