using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <typeparam name="T">ID</typeparam>
/// <typeparam name="U">파싱될 클래스</typeparam>
public class TableDuplicatedID<T, U> : Table<T, U>
    where U : ITableAvaialable<T>
{
    public TableDuplicatedID(string tableFileName) : base(tableFileName) { }

    /// <summary>
    /// Key: ID, Value: Class List
    /// </summary>
    protected new Dictionary<T, List<U>> datas;

    protected override void OnAssetLoaded(AsyncOperationHandle<TextAsset> handle)
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

            while (csvReader.Read())
            {
                var record = csvReader.GetRecord<U>();
                (record as ITableExtraLoadable)?.ExtraLoad(csvReader);

                if (datas.ContainsKey(record.TableID))
                    datas[record.TableID].Add(record);
                else
                    datas.Add(record.TableID, new() { record });
            }
        }
    }

    public new List<U> GetData(T id)
    {
        return datas[id];
    }
    public override List<U> GetDatas()
    {
        var list = new List<U>();

        foreach (var data in datas)
        {
            foreach (var item in data.Value)
            {
                list.Add(item);
            }
        }

        return list;
    }
}