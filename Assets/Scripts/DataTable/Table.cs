using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <typeparam name="T">ID</typeparam>
/// <typeparam name="U">파싱될 클래스</typeparam>
public class Table<T, U> where U : ITableAvaialable<T>
{
    protected readonly string fileName;

    public Table(string tableFileName)
    {
        fileName = tableFileName;
    }

    /// <summary>
    /// Key: ID, Value: Class
    /// </summary>
    protected Dictionary<T, U> datas;

    public AsyncOperationHandle<TextAsset> Load()
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>(fileName);
        handle.Completed += OnAssetLoaded;
        return handle;
    }

    protected virtual void OnAssetLoaded(AsyncOperationHandle<TextAsset> handle)
    {
        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            ErrorMessage(ERROR_TYPE.LOAD_FAILED, handle.Status.ToString());
            return;
        }

        datas = new();

        var textAsset = handle.Result;
        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csvReader.Read();
            csvReader.Read();

            var records = csvReader.GetRecords<U>();
            foreach (var record in records)
            {
                if (datas.ContainsKey(record.TableID))
                {
                    ErrorMessage(ERROR_TYPE.ID_DUPLICATED, record.TableID.ToString());
                    return;
                }

                datas.Add(record.TableID, record);
            }
        }
    }

    public U GetData(T id)
    {
        return datas[id];
    }

    public virtual List<U> GetDatas()
    {
        var list = new List<U>();

        foreach (var data in datas)
        {
            list.Add(data.Value);
        }

        return list;
    }


    /////////////////////////////////////////////////////////////////////////
    // ERROR MESSAGE ////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////

    protected enum ERROR_TYPE
    {
        ID_DUPLICATED,
        LOAD_FAILED
    }

    protected static readonly string formatIdDuplicated = "{0}에 중복된 ID {1}가 있습니다.";
    protected static readonly string formatLoadFailed = "{0}을 불러오지 못했습니다.\n{1}";

    protected void ErrorMessage(ERROR_TYPE type, string errorInfo)
    {
        switch (type)
        {
            case ERROR_TYPE.ID_DUPLICATED:
                Debug.LogError(string.Format(formatIdDuplicated, fileName, errorInfo));
                break;
            case ERROR_TYPE.LOAD_FAILED:
                Debug.LogError(string.Format(formatLoadFailed, fileName, errorInfo));
                break;
        }
    }
}