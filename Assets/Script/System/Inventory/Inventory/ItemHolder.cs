using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public abstract class ItemHolder : MonoBehaviour
{
    [SerializeField] protected InventorySO inventorySO;
    [SerializeField] protected ItemHolder extraHolder;
    [SerializeField] protected GameObject tempSlotDrag;
    [SerializeField] protected Transform drop;
    [SerializeField] protected ItemHolderType type;
    public Slot[] slots;

    protected InputSetting _input;
    protected Slot tempSlot;
    protected ItemSO tempItem;
    protected int tempAmount;
    private Slot mouseOverSlot;

    private RectTransform rect;
    private Image image;
    private bool mouseOver;

    protected void Awake()
    {
        tempSlotDrag.SetActive(false);
        rect = tempSlotDrag.GetComponent<RectTransform>();
        image = tempSlotDrag.GetComponent<Image>();
        AssignEvent();
    }

    private void OnEnable()
    {
        // Regiter Event
        EventDispatcher.SaveEvent += inventorySO.Save;
        EventDispatcher.LoadEvent += LoadFileSave;
        EventDispatcher.DeleteSaveDataEvent += inventorySO.Reset;
        EventDispatcher.ChangedEnduranceEvent += ChangedEndurance;
        EventDispatcher.ClearMapEvent += Clear;
    }

    private void Start()
    {
        _input = InputSetting.instance;
    }

    private void OnDisable()
    {
        EventDispatcher.SaveEvent -= inventorySO.Save;
        EventDispatcher.LoadEvent -= LoadFileSave;
        EventDispatcher.DeleteSaveDataEvent -= inventorySO.Reset;
        EventDispatcher.ChangedEnduranceEvent -= ChangedEndurance;
        EventDispatcher.ClearMapEvent -= Clear;
    }

    private void Clear()
    {
        inventorySO.Reset();
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].UpdateUI(-1, 0, 0);
        }
    }

    protected void Update()
    {
        MouseOver();
    }

    private void LoadFileSave()
    {
        inventorySO.Load();
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].UpdateUI(inventorySO.slotDatas[i].m_ID, inventorySO.slotDatas[i].m_Amount, inventorySO.slotDatas[i].m_Endurance);
        }
    }

    private void ChangedEndurance(int index, ItemHolderType type, float enduranceChange)
    {
        if (type == this.type)
            slots[index].UpdateEndurance(enduranceChange);
    }

    #region Inventory Method

    protected int CountEmptySlot
    {
        get
        {
            int counter = 0;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].data.m_ID == -1)
                    counter++;
            }
            return counter;
        }
    }

    public Slot GetSlotEmpty
    {
        get
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].data.m_ID == -1)
                    return slots[i];
            }
            return null;
        }
    }

    protected Slot GetSlotWithID(int id)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].data.m_ID == id && !slots[i].isFull)
                return slots[i];
        }
        return null;
    }

    protected int GetAmountItem(int id)
    {
        int amount = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].data.m_ID == id)
            {
                amount += slots[i].data.m_Amount;
            }
        }
        return amount;
    }

    protected void RemoveItem(int id, int amount)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            while (amount > 0)
            {
                var slot = GetSlotWithID(id);
                if (slot.data.m_Amount > amount)
                {
                    slot.UpdateUI(id, slot.data.m_Amount - amount, slot.data.m_Endurance);
                    amount = 0;
                }
                else
                {
                    amount -= slot.data.m_Amount;
                    slot.UpdateUI(id, 0, 0f);
                }
            }
        }
    }

    protected abstract void Swap(Slot slot1, Slot slot2);

    protected abstract void RightClickSlot(Slot slot);
    #endregion

    #region Slot Event

    private void AssignEvent()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            var slot = slots[i];
            var slotGO = slots[i].gameObject;
            slot.slotIndex = i;

            AddEventTrigger.AddEvent(slotGO, EventTriggerType.PointerEnter, delegate { OnEnter(slot); });
            AddEventTrigger.AddEvent(slotGO, EventTriggerType.PointerExit, delegate { OnExit(); });
            AddEventTrigger.AddEvent(slotGO, EventTriggerType.BeginDrag, delegate { OnBeginDrag(slot); });
            AddEventTrigger.AddEvent(slotGO, EventTriggerType.Drag, delegate { OnDrag(); });
            AddEventTrigger.AddEvent(slotGO, EventTriggerType.EndDrag, delegate { OnEndDrag(); });
        }
    }

    private void CreateTempSlot(Slot slot)
    {
        tempSlotDrag.transform.position = _input.position;
        tempSlotDrag.SetActive(true);
        rect.sizeDelta = new Vector2(80f * Screen.width / 1920f, 80f * Screen.height / 1080f);
        image.sprite = inventorySO.database.GetItemWithID(slot.data.m_ID).iconItem;
    }

    private void MouseOver()
    {
        if (!mouseOver || mouseOverSlot.data.m_ID == -1)
        {
            mouseOver = false;
            return;
        }
        var item = inventorySO.database.GetItemWithID(mouseOverSlot.data.m_ID);
        ShowToolTip(item);
        if (_input.rightClick)
        {
            _input.rightClick = false;
            RightClickSlot(mouseOverSlot);
        }
    }

    private void OnEnter(Slot slot)
    {
        MouseData.slot2 = slot;
        if (slot.data.m_ID == -1)
            return;
        mouseOver = true;
        mouseOverSlot = slot;
    }

    protected abstract void ShowToolTip(ItemSO item);

    private void OnExit()
    {
        mouseOver = false;
        MouseData.slot2 = null;
        mouseOverSlot = null;
        EventDispatcher.HideToolTip();
    }

    private void OnBeginDrag(Slot slot)
    {
        if (slot == null)
            return;
        if (slot.data.m_ID == -1)
            return;
        MouseData.slot1 = slot;
        CreateTempSlot(slot);
        EventDispatcher.EnableStateBin(true);
    }

    private void OnDrag()
    {
        if (MouseData.slot1 == null)
            return;
        tempSlotDrag.transform.position = _input.position;
    }


    private void OnEndDrag()
    {
        if (MouseData.slot1 == null)
            return;
        EventDispatcher.DropItem(MouseData.slot1, MouseData.slot1.data.m_Amount, drop.position);
        Swap(MouseData.slot1, MouseData.slot2);
        tempSlotDrag.SetActive(false);
        MouseData.slot1 = null;
        MouseData.slot2 = null;
        EventDispatcher.EnableStateBin(false);
    }
    #endregion

    private void OnApplicationQuit()
    {
        inventorySO.Reset();
    }
}

public enum ItemHolderType
{
    Inventory,
    Equipment
}
public static class MouseData
{
    public static Slot slot1;
    public static Slot slot2;
}