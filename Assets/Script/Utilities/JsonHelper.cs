using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JsonHelper
{
    public static List<T> FromJsonList<T>(string json)
    {
        Wapper<T> wapper = JsonUtility.FromJson<Wapper<T>>(json);
        return wapper.itemList;
    }

    public static string ToJson<T>(List<T> list)
    {
        Wapper<T> wapper = new Wapper<T>();
        wapper.itemList = list;
        return JsonUtility.ToJson(wapper);
    }

    public static T[] FromJson<T>(string json)
    {
        Wapper<T> wapper = JsonUtility.FromJson<Wapper<T>>(json);
        return wapper.itemArray;
    }

    public static string ToJon<T>(T[] array)
    {
        Wapper<T> wapper = new Wapper<T>();
        wapper.itemArray = array;
        return JsonUtility.ToJson(wapper);
    }
}

[System.Serializable]
public class Wapper<T>
{
    public T[] itemArray;
    public List<T> itemList;
}
