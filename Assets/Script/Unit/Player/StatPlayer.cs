using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StatPlayer : MonoBehaviour, IModifierStat 
{
    [SerializeField] private StatSystem[] healthStats;
    [SerializeField] private BuffItemStat[] baseBuffStats;
    public float currentEnergy { get; set; }
    [SerializeField] private float maxEnergy;

    private BuffItemStat[] buffStats;
    private string saveJson;

    private void OnEnable()
    {
        for (int i = 0; i < healthStats.Length; i++)
        {
            healthStats[i].unit = transform;
        }
        buffStats = baseBuffStats;
        EventDispatcher.LoadEvent += LoadStatPlayer;
        EventDispatcher.SaveEvent += SaveStatPlayer;

        EventDispatcher.OutOfStatEvent += OutOfHealth;
        EventDispatcher.ChangedStatEvent += ChangedHealth;

        for (int i = 1; i < healthStats.Length; i++)
        {
            EventDispatcher.OutOfStatEvent += OutOfStatBody;
            EventDispatcher.ChangedStatEvent += ChangedStatBody;
        }

        EventDispatcher.ChangeEquipmentEvent += ChangeStat;
        EventDispatcher.ChangedEnergyEvent += ChangedEnergy;

        EventDispatcher.GetStatEvent += GetStat;
        EventDispatcher.ModifierStatEvent += ModifierStat;
    }

    private void OnDisable()
    {
        EventDispatcher.LoadEvent -= LoadStatPlayer;
        EventDispatcher.SaveEvent -= SaveStatPlayer;

        EventDispatcher.OutOfStatEvent -= OutOfHealth;
        EventDispatcher.ChangedStatEvent -= ChangedHealth;

        for (int i = 1; i < healthStats.Length; i++)
        {
            EventDispatcher.OutOfStatEvent -= OutOfStatBody;
            EventDispatcher.ChangedStatEvent -= ChangedStatBody;
        }

        EventDispatcher.ChangeEquipmentEvent += ChangeStat;
        EventDispatcher.ChangedEnergyEvent -= ChangedEnergy;

        EventDispatcher.GetStatEvent -= GetStat;
        EventDispatcher.ModifierStatEvent -= ModifierStat;
    }

    private void ModifierStat(BuffStat stat, float value)
    {
        Modifier(stat, value);
    }

    private void ChangedEnergy(float value, bool state)
    {
        if (state)
        {
            currentEnergy += value;
            if (currentEnergy >= maxEnergy) currentEnergy = maxEnergy;

        }
        else
        {
            currentEnergy -= value;
            if (currentEnergy <= 0)
            {
                currentEnergy = 0;
                EventDispatcher.OutOfEnergy();
            }
        }
        EventDispatcher.ChangedFillBar("Energy", currentEnergy / maxEnergy);
    }

    private float GetStat(BuffStat stat)
    {
        for (int i = 0; i < buffStats.Length; i++)
        {
            if(stat == buffStats[i].stat)
            {
                return buffStats[i].value;
            }
        }
        return 0;
    }

    #region Health Stat
    private void OutOfHealth(StatSystem stat, Transform player)
    {
        if (transform != player)
            return;
        if (stat == healthStats[0])
            Debug.Log("Death");
    }

    private void ChangedHealth(StatSystem stat)
    {
        if (stat == healthStats[0])
        {
            Debug.Log("Hurt");
        }
    }

    private void OutOfStatBody(StatSystem stat, Transform player)
    {
        if (transform != player)
            return;
        if (healthStats[1] == stat || healthStats[2] == stat)
            Debug.Log("Out of " + stat.stat);
    }

    private void ChangedStatBody(StatSystem stat)
    {
        if (healthStats[1] == stat || healthStats[2] == stat)
        {
            Debug.Log("Reduce " + stat.stat);
        }
    }

    public void Modifier(BuffStat stat, float value)
    {
        for (int i = 0; i < healthStats.Length; i++)
        {
            if(stat == healthStats[i].stat)
            {
                healthStats[i].ModifierStat(value);
            }
        }
    }

    #endregion

    #region Buff Stat
    private void ChangeStat(EquipmentSO lastEquipment, EquipmentSO newEquipment)
    {
        if (lastEquipment != null)
            for (int i = 0; i < buffStats.Length; i++)
            {
                for (int j = 0; j < lastEquipment.stats.Length; j++)
                {
                    if (buffStats[i].stat == lastEquipment.stats[j].stat)
                        buffStats[i].UpdateStat(buffStats[i].value - lastEquipment.stats[j].value);
                }
            }
        if (newEquipment != null)
            for (int i = 0; i < buffStats.Length; i++)
            {
                for (int j = 0; j < newEquipment.stats.Length; j++)
                {
                    if (buffStats[i].stat == newEquipment.stats[j].stat)
                        buffStats[i].UpdateStat(buffStats[i].value + newEquipment.stats[j].value);
                }
            }
    }
    #endregion

    #region Save Load Stat
    private void SaveStatPlayer()
    {
        saveJson = JsonHelper.ToJon(healthStats);
        PlayerPrefs.SetString("Save_Stats", saveJson);
        PlayerPrefs.Save();
    }
    private void LoadStatPlayer()
    {
        if (!PlayerPrefs.HasKey("Save_Stats"))
        {
            for (int i = 0; i < healthStats.Length; i++)
            {
                healthStats[i].ResetStat();
            }
            return;
        }
        saveJson = PlayerPrefs.GetString("Save_Stats");
        healthStats = JsonHelper.FromJson<StatSystem>(saveJson);

    }
    #endregion
}