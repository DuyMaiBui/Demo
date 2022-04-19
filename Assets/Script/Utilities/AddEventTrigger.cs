using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class AddEventTrigger
{
    public static void AddEvent(GameObject _gameObject, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = _gameObject.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }
}
