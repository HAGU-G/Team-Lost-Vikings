﻿using UnityEngine;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameSetting : ScriptableObject
{
#if UNITY_EDITOR
    private static string[] fileDirectory = {
        "Assets",
        "Settings",
        "Resources"
    };

    [MenuItem("Window/게임 설정", false, 1230)]
    private static void SelectSettingAsset()
    {
        Selection.activeObject = Instance;
    }
#endif

    #region INSTANCE
    private static string fileName = "Game Setting";

    private static GameSetting _instance;
    public static GameSetting Instance
    {

        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<GameSetting>(fileName);
#if UNITY_EDITOR
                if (_instance == null)
                {
                    var filePath = new StringBuilder(fileDirectory[0]);
                    for (int i = 1; i < fileDirectory.Length; i++)
                    {
                        if (!AssetDatabase.IsValidFolder(string.Concat(filePath, "/", fileDirectory[i])))
                        {
                            AssetDatabase.CreateFolder(filePath.ToString(), fileDirectory[i]);
                        }
                        filePath.Append(string.Concat("/", fileDirectory[i]));
                    }
                    filePath.Append(string.Concat("/", fileName));

                    _instance = CreateInstance<GameSetting>();
                    AssetDatabase.CreateAsset(_instance, string.Concat(filePath.ToString(), ".asset"));
                }
#endif
            }
            return _instance;
        }
    }
    #endregion

    //////////////////////////////////////////////////////////////////////////////////////////////
    // 시간 설정 /////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////
    public string serverURI = "google.com";
    public enum SYNC_INTERVAL_TYPE
    {
        MINUTE,
        HOUR
    }
    public SYNC_INTERVAL_TYPE syncIntervalType = SYNC_INTERVAL_TYPE.MINUTE;

    [Range(1, 60)]
    public int syncInterval = 30;

    [Range(4, 600)]
    public int syncLimitSeconds = 4;




    //////////////////////////////////////////////////////////////////////////////////////////////
    // 타일 설정 /////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////

    [Range(0f, 2f)]
    public float ellipseRatio = 0.75f;




    //////////////////////////////////////////////////////////////////////////////////////////////
    // AI 설정 ///////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////

    [Range(0f, 60f)]
    public float monsterRoamTime = 2f;

    [Range(0f, 1f)]
    public float returnHPRaito = 0.2f;
    [Range(0f, 1f)]
    public float returnStaminaRaito = 0.2f;
    [Range(0f, 1f)]
    public float returnStressRaito = 0.2f;

    [Range(0, 100)]
    public int staminaReduceAmount = 1;
    [Range(0, 100)]
    public int stressReduceAmount = 1;



    //////////////////////////////////////////////////////////////////////////////////////////////
    // 사냥터 설정 ///////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////

    [Range(0f, 600f)]
    public float monsterSpawnInterval = 6f;
}
