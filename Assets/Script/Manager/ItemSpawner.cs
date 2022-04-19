using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private ItemSO[] natureItem;
    [SerializeField] private List<ItemSaveData> saveList;
    private string saveJson;

    private void OnEnable()
    {
        EventDispatcher.SaveEvent += SaveItem;
        EventDispatcher.LoadEvent += LoadItem;
        EventDispatcher.SetItemParentEvent += SetItemParent;
        EventDispatcher.GenerateNatureItemEvent += GenerateNatureItem;

        EventDispatcher.ClearMapEvent += ClearItem;
    }

    private void OnDisable()
    {
        EventDispatcher.SaveEvent -= SaveItem;
        EventDispatcher.LoadEvent -= LoadItem;
        EventDispatcher.SetItemParentEvent -= SetItemParent;
        EventDispatcher.GenerateNatureItemEvent -= GenerateNatureItem;

        EventDispatcher.ClearMapEvent -= ClearItem;
    }

    private void GenerateNatureItem(TileData data)
    {
        int random = Random.Range(0, 99);
        if (random < 70)
            return;
        int randomAmount = Random.Range(0, 5);
        for (int i = 0; i < randomAmount; i++)
        {
            int randomItem = Random.Range(0, natureItem.Length);
            Vector3 randomPosition = new Vector3(Random.Range(data.position.x - 4f, data.position.x + 4f), data.position.y + 1f, Random.Range(data.position.z - 4f, data.position.z + 4f));
            Vector3 randomRotation = new Vector3(0, Random.Range(0f, 180f), 0);
            var go = ObjectPoolManager.instance.InstanceGameObject(natureItem[randomItem].itemName, transform);
            go.transform.position = randomPosition;
            go.transform.eulerAngles = randomRotation;
        }
    }

    private void SaveItem()
    {
        StartCoroutine(Save());
    }

    private void LoadItem()
    {
        if (!PlayerPrefs.HasKey("Save_Items"))
            return;
        saveJson = PlayerPrefs.GetString("Save_Items");
        saveList = JsonHelper.FromJsonList<ItemSaveData>(saveJson);
        for (int i = 0; i < saveList.Count; i++)
        {
            var item = ObjectPoolManager.instance.InstanceGameObject(saveList[i].item.itemName, saveList[i].position, saveList[i].rotation, transform);
            item.GetComponent<ItemGO>().SetAmount(saveList[i].amount);
        }
    }

    IEnumerator Save()
    {
        EventDispatcher.SaveItemData(saveList);
        yield return null;
        saveJson = JsonHelper.ToJson(saveList);
        Debug.Log(saveJson);
        PlayerPrefs.SetString("Save_Items", saveJson);
        PlayerPrefs.Save();
    }

    private void SetItemParent(GameObject unit)
    {
        unit.transform.SetParent(transform, true);
    }

    private void ClearItem()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            ObjectPoolManager.instance.DeactiveGameObject(transform.GetChild(i).name.Replace("(Clone)", "").Trim(), transform.GetChild(i).gameObject);
        }
    }
}
