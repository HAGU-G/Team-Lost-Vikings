using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDict<T>
{
    public List<SerializableData<T>> data;
    public int Count { get { return data.Count; } }

    public SerializableData<T> this[int index]
    {
        get
        {
            if (index >= 0 && index < data.Count)
            {
                return data[index];
            }
            else
            {
                throw new IndexOutOfRangeException("인덱스가 범위를 벗어났습니다.");
            }
        }
        set
        {
            if (index >= 0 && index < data.Count)
            {
                data[index] = value;
            }
            else
            {
                throw new IndexOutOfRangeException("인덱스가 범위를 벗어났습니다.");
            }
        }
    }

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
