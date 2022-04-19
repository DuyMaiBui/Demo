using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance;

    [SerializeField] private ObjectPool[] pools;

    public Dictionary<string, ObjectPool> poolDictionary = new Dictionary<string, ObjectPool>();

    private void Awake()
    {
        SetDictionary();
    }

    public void SetDictionary()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        for (int i = 0; i < pools.Length; i++)
        {
            poolDictionary.Add(pools[i].m_GameObject.name, pools[i]);
        }
    }

    private GameObject CreateGameObject(string key)
    {
        if (poolDictionary[key].m_Queue.Count > 0)
        {
            var go = poolDictionary[key].m_Queue.Dequeue();
            go.SetActive(true);
            return go;
        }
        else
        {
            var go = Instantiate(poolDictionary[key].m_GameObject);
            go.SetActive(true);
            return go;
        }
    }

    public IEnumerator DeactiveGameObject(string key, GameObject _gameObject, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        _gameObject.SetActive(false);
        poolDictionary[key].m_Queue.Enqueue(_gameObject);
    }

    public void DeactiveGameObject(string key, GameObject _gameObject)
    {
        _gameObject.SetActive(false);
        poolDictionary[key].m_Queue.Enqueue(_gameObject);
    }

    public GameObject InstanceGameObject(string key)
    {
        return CreateGameObject(key);
    }

    public GameObject InstanceGameObject(string key, Transform parent)
    {
        var go = CreateGameObject(key);
        go.transform.parent = parent;
        return go;
    }

    public GameObject InstanceGameObject(string key, Vector3 position, Quaternion rotate)
    {
        var go = CreateGameObject(key);
        go.transform.position = position;
        go.transform.rotation = rotate;
        return go;
    }

    public GameObject InstanceGameObject(string key, Vector3 position, Quaternion rotate, Transform parent)
    {
        var go = CreateGameObject(key);
        go.transform.position = position;
        go.transform.rotation = rotate;
        go.transform.parent = parent;
        return go;
    }

}

[System.Serializable]
public class ObjectPool
{
    public Queue<GameObject> m_Queue = new Queue<GameObject>();
    public GameObject m_GameObject;
}
