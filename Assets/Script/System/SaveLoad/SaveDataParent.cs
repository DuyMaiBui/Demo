using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataParent : MonoBehaviour
{
    public GameObject environment;

    private string saveJson;
    public SaveDataChild[] saveDataChilds;

    public string GetSaveData
    {
        get
        {
            GetChidData();
            return saveJson;
        }
    }

    public void GetChidData()
    {
        saveJson = string.Empty;
        saveDataChilds = new SaveDataChild[transform.childCount];
        saveJson += transform.childCount + " ";
        for (int i = 0; i < saveDataChilds.Length; i++)
        {
            saveDataChilds[i].position = transform.GetChild(i).position;
            saveDataChilds[i].rotate = transform.GetChild(i).rotation;
        }
        saveJson += JsonHelper.ToJon(saveDataChilds);
        saveJson += "/";
    }

    public void SetSaveData(string saveData)
    {
        string[] saveDataArray = saveData.Split(" ");
        saveDataChilds = JsonHelper.FromJson<SaveDataChild>(saveDataArray[1]);
        LoadEnvironment();
    }

    public void LoadEnvironment()
    {
        if(saveDataChilds.Length > 0)
            foreach (var data in saveDataChilds)
            {
                Instantiate(environment, data.position, data.rotate, transform);
            }
    }
}

[System.Serializable]
public struct SaveDataChild
{
    public Vector3 position;
    public Quaternion rotate;

    public void UpdateData(Vector3 position, Quaternion rotate)
    {
        this.position = position;
        this.rotate = rotate;
    }
}