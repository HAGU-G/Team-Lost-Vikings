using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDict<T>
{
    public List<SerializableData<T>> data;
    private Dictionary<Vector2Int, T> diction = new();

    public Dictionary<Vector2Int, T> GetDictionary()
    {
        for(int i = 0; i < data.Count; ++i)
        {
            diction.Add(data[i].key, data[i].value);
        }
        return diction;
    }
}

[Serializable]
public class SerializableData<T>
{
    public Vector2Int key;
    public T value;
}
