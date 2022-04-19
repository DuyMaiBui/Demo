using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatUnit : MonoBehaviour, IModifierStat
{
    [Header("Stat")]
    [SerializeField] private StatSystem statBase;
    [SerializeField] private float maxHealth;
    [SerializeField] private SpawnObject[] spawns;



    void OnEnable()
    {
        statBase = new StatSystem(BuffStat.Health, maxHealth);
        statBase.unit = transform;
        EventDispatcher.OutOfStatEvent += Death;
        EventDispatcher.ChangedStatEvent += ChangedHealth;
    }

    void OnDisable()
    {
        EventDispatcher.OutOfStatEvent -= Death;
        EventDispatcher.ChangedStatEvent -= ChangedHealth;
    }

    private void Death(StatSystem stat, Transform unit)
    {
        if (transform == unit)
        {
            for (int i = 0; i < spawns.Length; i++)
            {
                for (int j = 0; j < spawns[i].amount; j++)
                {
                    Vector3 randomOffset = new Vector3(Random.Range(-spawns[i].offset, spawns[i].offset), spawns[i].offset, Random.Range(-spawns[i].offset, spawns[i].offset));
                    GameObject go = ObjectPoolManager.instance.InstanceGameObject(spawns[i].key, transform.position + randomOffset, transform.rotation);
                    EventDispatcher.SetItemParent(go);
                }
            }
            ObjectPoolManager.instance.DeactiveGameObject(transform.parent.name, gameObject);
        }
    }

    private void ChangedHealth(StatSystem stat)
    {
        if (stat != this.statBase)
            return;
    }

    public void Modifier(BuffStat stat, float value)
    {
        if (stat == statBase.stat)
            statBase.ModifierStat(value);
    }
}
[System.Serializable]
public struct SpawnObject
{
    public string key;
    public int amount;
    public float offset;
}