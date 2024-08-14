using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using CurrentSave = SaveDataV1;

public static class SaveManager
{
    private static readonly string fileDirectory = $"{Application.persistentDataPath}/save";
    private static readonly string fileName = "CBTSAVEFILE";
    private static readonly string key = "89f0d73j038fjje0";

    private static SaveData saveData = null;
    private static byte[] encryptedSaveData = null;

    public static void SaveGame()
    {
#if UNITY_EDITOR
        if (!GameSetting.Instance.useSaveDataWhenEditor)
            return;
#endif
        if (saveData == null)
            saveData = new CurrentSave();

        var save = saveData as CurrentSave;

        //Version 1
        save.unitManager = GameManager.unitManager;
        save.playerManager = GameManager.playerManager;
        save.itemManager = GameManager.itemManager;
        save.questManager = GameManager.questManager;
        save.dialogManager = GameManager.dialogManager;

        save.huntZones.Clear();
        foreach (var huntZoneInfo in GameManager.huntZoneManager.HuntZones)
        {
            save.huntZones.Add(huntZoneInfo.Value.Info);
        }
        save.UnitDeployment = GameManager.huntZoneManager.UnitDeployment;

        for (int i = 0; i < GameManager.villageManager.constructedBuildings.Count; ++i)
        {
            var building = GameManager.villageManager.constructedBuildings[i].GetComponent<Building>();
            Debug.Log(GameManager.villageManager.constructedBuildings[i]);
            var tileId = building.standardTile.tileInfo.id;
            var structureId = building.StructureId;
            Debug.Log($"{structureId},{tileId}");

            if(!save.buildings.ContainsKey(tileId))
                save.buildings.Add(tileId, structureId);
        }

        save.buildingUpgrade.Clear();
        foreach (var building in GameManager.villageManager.constructedBuildings)
        {
            var up = building.GetComponent<BuildingUpgrade>();
            var id = building.GetComponent<Building>().StructureId;
            save.buildingUpgrade.Add(id,up == null ? 0 : up.currentGrade);
        }


        SaveFile();
    }

    public static void LoadGame()
    {
#if UNITY_EDITOR
        if (!GameSetting.Instance.useSaveDataWhenEditor)
            return;
#endif
        if (saveData == null)
            saveData = new CurrentSave();

        var load = LoadFile();
        if (load == null)
            return;

        while (load.version != saveData.version)
        {
            if (load.version < saveData.version)
                load = load.VersionUp();
            else if (load.version > saveData.version)
                load = load.VersionDown();
        }

        saveData = load;
        var save = saveData as CurrentSave;

        //Version 1
        GameManager.unitManager = save.unitManager;
        GameManager.playerManager = save.playerManager;
        GameManager.itemManager = save.itemManager;
        GameManager.questManager = save.questManager;
        GameManager.dialogManager = save.dialogManager;

        foreach (var huntZoneInfo in save.huntZones)
        {
            GameManager.huntZoneManager.HuntZones[huntZoneInfo.HuntZoneNum].Info = huntZoneInfo;
        }
        GameManager.huntZoneManager.UnitDeployment = save.UnitDeployment;


        foreach (var key in save.buildings.Keys)
        {
            save.buildings.TryGetValue(key, out int structureId);
            var obj = GameManager.villageManager.objectList[structureId];
            var tile = GameManager.villageManager.gridMap.GetTile(key.x, key.y);

            var constructedObj = 
            GameManager.villageManager.constructMode.construct.PlaceBuilding(obj, tile, GameManager.villageManager.gridMap);

            var up = constructedObj.GetComponent<BuildingUpgrade>();

            if (up == null)
                continue;

            var upId = DataTableManager.buildingTable.GetData(structureId).UpgradeId;

            up.currentGrade = save.buildingUpgrade[structureId];
            up.SetBuildingUpgrade();
            //up.GetComponent<StatUpgradeBuilding>()?.RiseStat();
            up.Upgrade(true);

            GameManager.villageManager.SetDevelopText(false);
        }
    }

    private static void SaveFile()
    {
        if (!Directory.Exists(fileDirectory))
            Directory.CreateDirectory(fileDirectory);

        var path = Path.Combine(fileDirectory, fileName);
        using (var stringWriter = new StringWriter())
        {
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                var serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.TypeNameHandling = TypeNameHandling.All;
                serializer.Converters.Add(new Vector2IntConverter());
                serializer.Serialize(jsonWriter, saveData);
            }

            File.WriteAllText(path, stringWriter.ToString());

            //byte[] bytes = System.Text.Encoding.UTF8.GetBytes(stringWriter.ToString());
            //ICryptoTransform cryptoTransform = NewRijndaeManaged().CreateEncryptor();
            //encryptedSaveData = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
            //File.WriteAllBytes(path, encryptedSaveData);
        }
    }

    private static SaveData LoadFile(byte[] data = null)
    {
        if (data == null)
        {
            if (!Directory.Exists(fileDirectory))
                return null;

            var path = Path.Combine(fileDirectory, fileName);
            if (!File.Exists(path))
                return null;

            encryptedSaveData = File.ReadAllBytes(path);

        }
        else
        {
            encryptedSaveData = data;
        }

        SaveData load = new CurrentSave();

        using (var reader = new JsonTextReader(new StringReader(File.ReadAllText(Path.Combine(fileDirectory, fileName)))))
        {
            var serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.TypeNameHandling = TypeNameHandling.All;
            serializer.Converters.Add(new Vector2IntConverter());
            load = serializer.Deserialize<SaveData>(reader);
        }
        //ICryptoTransform cryptoTransform2 = NewRijndaeManaged().CreateDecryptor();
        //byte[] result = cryptoTransform2.TransformFinalBlock(encryptedSaveData, 0, encryptedSaveData.Length);
        //using (var reader = new JsonTextReader(new StringReader(System.Text.Encoding.UTF8.GetString(result))))
        //{
        //    var serializer = new JsonSerializer();
        //    serializer.Formatting = Formatting.Indented;
        //    serializer.TypeNameHandling = TypeNameHandling.All;
        //    load = serializer.Deserialize<SaveData>(reader);
        //}

        return load;
    }

    private static RijndaelManaged NewRijndaeManaged()
    {
        byte[] keys = System.Text.Encoding.UTF8.GetBytes(key);
        byte[] newKeys = new byte[keys.Length];
        System.Array.Copy(keys, 0, newKeys, 0, keys.Length);

        RijndaelManaged rijndaelManaged = new RijndaelManaged();
        rijndaelManaged.Key = newKeys;
        rijndaelManaged.Mode = CipherMode.ECB;
        rijndaelManaged.Padding = PaddingMode.PKCS7;

        return rijndaelManaged;
    }
}
