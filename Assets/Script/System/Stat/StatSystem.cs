using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StatSystem
{
    public Transform unit;
    public string barName;
    public BuffStat stat;
    public float maxValue;
    public float currentValue;

    public StatSystem()
    {
        stat = BuffStat.Health;
        maxValue = currentValue = 100;
    }

    public StatSystem(BuffStat stat, float maxValue)
    {
        this.stat = stat;
        this.maxValue = this.currentValue = maxValue;
    }

    public void ResetStat()
    {
        currentValue = maxValue = 100;
    }

    public float GetStatValue
    {
        get
        {
            return currentValue;
        }
    }

    public float GetPercent
    {
        get
        {
            return currentValue / maxValue;
        }
    }

    public void ModifierStat(float value)
    {
        currentValue += value;
        if (currentValue >= maxValue)
            currentValue = maxValue;
        if (currentValue <= 0)
        {
            currentValue = 0;
            EventDispatcher.OutOfStat(this, unit);
        }
        EventDispatcher.ChangedStat(this);
    }
}
