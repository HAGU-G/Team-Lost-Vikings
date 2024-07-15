using UnityEngine;
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
    private static string fileName = "UtilitySetting";

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
    public int syncInterval = 30;
    public int syncLimitSeconds = 4;




    //////////////////////////////////////////////////////////////////////////////////////////////
    // 타일 설정 /////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////
    public float ellipseRatio = 0.75f;




    //////////////////////////////////////////////////////////////////////////////////////////////
    // AI 설정 ///////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////
    public float monsterRoamTime = 6f;
}
