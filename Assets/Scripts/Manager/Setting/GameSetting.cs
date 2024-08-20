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
    [Header("에디터 설정")]

    [Tooltip("에디터에서 저장/불러오기 사용")]
    public bool useSaveDataWhenEditor = false;








    //////////////////////////////////////////////////////////////////////////////////////////////
    [Header("시간 설정")]

    public string serverURI = "google.com";
    public enum SYNC_INTERVAL_TYPE
    {
        MINUTE,
        HOUR
    }
    public SYNC_INTERVAL_TYPE syncIntervalType = SYNC_INTERVAL_TYPE.MINUTE;

    [Tooltip("갱신 주기")]
    [Range(1, 60)] public int syncInterval = 30;
    [Range(4, 60)] public int syncLimitSeconds = 4;




    //////////////////////////////////////////////////////////////////////////////////////////////
    [Header("타일 설정")]

    [Tooltip("충돌용 타원 비율")]
    [Range(0f, 2f)] public float ellipseRatio = 0.75f;

    [Tooltip("타일 가로 세로 비율")]
    public float tileXY = 1.8f;



    //////////////////////////////////////////////////////////////////////////////////////////////
    [Header("유닛 설정")]

    [Range(0f, 1f)] public float returnHPRaito = 0.2f;
    [Range(0f, 1f)] public float returnStaminaRaito = 0.2f;
    [Range(0f, 1f)] public float returnStressRaito = 0.2f;

    public int staminaReduceAmount = 1;
    public int stressReduceAmount = 1;

    public int overrollUltraRare = 231;
    public int overrollSuperRare = 121;
    public int overrollRare = 91;
    public int overrollNormal = 51;

    public float autoGachaSeconds = 90f;
    public int autoGachaMaxCount = 99;

    [Tooltip("캐릭터 보유 수 기본값")]
    public int defaultUnitLimit = 4;


    [Tooltip("배회 경로 재탐색 시간")]
    public float idleRerouteTime = 3f;
    [Tooltip("공격 대상 재탐색 시간")]
    public float traceRerouteTime = 20f;

    //////////////////////////////////////////////////////////////////////////////////////////////
    [Header("투사체 설정")]

    public float projectileSize = 0.5f;
    [Tooltip("평타 화살 투사체 속도")]
    public float defaultBowProjectileSpeed = 4f;
    [Tooltip("평타 마법 투사체 속도")]
    public float defaultMagicProjectileSpeed = 4f;
    [Tooltip("평타 화살 투사체 이펙트 이름")]
    public string arrowEffectName = "BowProjectile";
    [Tooltip("평타 마법 투사체 이펙트 이름")]
    public string magicEffectName = "MagicProjectile";

    //////////////////////////////////////////////////////////////////////////////////////////////
    [Header("기타 설정")]

    public int goldID = 8000001;
    public string gachaPrefabName = "Char_Gacha";
    public string touchEffectName = string.Empty;
    public float touchEffectScale = 1f;
    public Color hitEffectColor = Color.red;
}
