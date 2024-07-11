using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public static class SyncedTime
{
    private static SyncedTimeSetting setting = SyncedTimeSetting.Instance;

    public static int CorrectionSeconds { get; private set; }
    public static int CorrectionMilliSeconds { get; private set; }
    public static long SecondCorrectionTick { get; private set; }

    /// <summary>
    /// 마지막으로 동기화 한 시간 (서버)
    /// </summary>
    public static DateTime LastSyncedTimeLocal { get; private set; } = DateTime.MinValue;
    public static DateTime LastSyncedTimeServer { get; private set; } = DateTime.MinValue;
    public static float LastSyncedTimeUnity { get; private set; }
    public static bool IsSynced { get; private set; }
    public static bool IsMillisecondSynced { get; private set; }
    public static bool IsSyncing { get; private set; }

    private static UnityWebRequest webRequest = null;
    private static CoroutineObject autoSyncCoroutine = null;

    /// <summary>
    /// 가장 마지막에 동기화된 시간을 반환.
    /// 동기화가 안됐을 경우 로컬 시간을 반환. (동기화 시도)
    /// </summary>
    public static DateTime Now
    {
        get
        {
            if (!IsSynced && IsNetworkReachable())
                Sync();

            return Correction(Time.realtimeSinceStartup, LastSyncedTimeUnity, LastSyncedTimeServer,
                CorrectionSeconds, CorrectionMilliSeconds, SecondCorrectionTick);
        }
    }

    private static DateTime Correction(float unityTime, float lastSyncedTimeUnity, DateTime lastSyncedTimeServer,
        int correctionSeconds, int correctionMilliseconds, long secondCorrectionTick)
    {
        var delta = unityTime - lastSyncedTimeUnity;

        return lastSyncedTimeServer
        .AddSeconds(correctionSeconds + Mathf.FloorToInt(delta))
        .AddMilliseconds(correctionMilliseconds + Mathf.FloorToInt(delta % 1f * 1000f))
        .AddTicks(secondCorrectionTick);
    }

    public static void AutoSync(bool value)
    {
        if (setting.syncInterval == 0f)
        {
            Debug.LogWarning("시간 동기화 주기가 0입니다. 설정을 확인해주세요.");
            return;
        }

        if (!IsNetworkReachable())
        {
            Debug.LogWarning("네트워크에 연결할 수 없습니다.");
            return;
        }

        if (value)
        {
            if (autoSyncCoroutine == null)
            {
                CoroutineObject.CreateCorutine(CoAutoSync());
            }
            else if (autoSyncCoroutine.IsStopped)
            {
                autoSyncCoroutine.StartCo(CoAutoSync());
            }
        }
        else
        {
            autoSyncCoroutine.StopCo();
        }
    }

    private static IEnumerator CoAutoSync()
    {
        while (true)
        {
            Sync();
            switch (setting.syncIntervalType)
            {
                case SyncedTimeSetting.SYNC_INTERVAL_TYPE.MINUTE:
                    yield return new WaitForSecondsRealtime(setting.syncInterval * 60f);
                    break;
                case SyncedTimeSetting.SYNC_INTERVAL_TYPE.HOUR:
                    yield return new WaitForSecondsRealtime(setting.syncInterval * 3600f);
                    break;
            }
        }
    }

    public static void Sync()
    {
        Debug.Log("시간 동기화 시도");

        if (!IsNetworkReachable())
        {
            Debug.LogWarning("네트워크에 연결할 수 없습니다.");
            return;
        }

        if (IsSyncing || webRequest != null)
        {
            Debug.LogWarning("이미 동기화 시도 중입니다.");
            return;
        }

        IsSyncing = true;

        var requestTime = Time.realtimeSinceStartup;
        webRequest = UnityWebRequest.Get(setting.serverURI);

        webRequest.SendWebRequest().completed += operation =>
        {
            var newUnityTime = Time.realtimeSinceStartup;
            if (webRequest.error != null)
            {
                Error(webRequest.error);
                IsSyncing = false;
            }
            else
            {
                if (DateTime.TryParse(webRequest.GetResponseHeader("date"), out var result))
                {
                    var responseDeltaTime = (newUnityTime - requestTime) / 2f;

                    var newServerTime = result.ToLocalTime();
                    var newCorrectionSeconds = -Mathf.FloorToInt(responseDeltaTime);
                    var newCorrectionMilliSeconds = -Mathf.FloorToInt(responseDeltaTime % 1f * 1000f);

                    if (!IsMillisecondSynced && IsSynced)
                    {
                        var newNow = Correction(0f, newUnityTime, newServerTime,
                            newCorrectionSeconds, newCorrectionMilliSeconds, 0).Ticks;
                        var lastNow = Correction(0f, LastSyncedTimeUnity, LastSyncedTimeServer,
                            CorrectionSeconds, CorrectionMilliSeconds, SecondCorrectionTick).Ticks;

                        var serverDiff = newServerTime.Ticks - LastSyncedTimeServer.Ticks;
                        SecondCorrectionTick = newNow - lastNow - serverDiff;
                        IsMillisecondSynced = true;
                    }

                    LastSyncedTimeUnity = newUnityTime;
                    LastSyncedTimeServer = newServerTime;
                    CorrectionSeconds = newCorrectionSeconds;
                    CorrectionMilliSeconds = newCorrectionMilliSeconds;


                    IsSynced = true;
                    IsSyncing = false;
                }
                else
                {
                    Error("헤더를 찾을 수 없음");
                    IsSyncing = false;
                }
            }
            webRequest.Dispose();
            webRequest = null;
        };
    }

    private static void Error(string message)
    {
        Debug.LogError(message);
    }

    private static bool IsNetworkReachable()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }
}