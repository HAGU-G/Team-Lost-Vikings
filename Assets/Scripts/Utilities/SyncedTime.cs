using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public static class SyncedTime
{
    private static SyncedTimeSetting setting = SyncedTimeSetting.Instance;

    public static long CorrectionTick { get; private set; }

    /// <summary>
    /// 마지막으로 동기화 한 시간 (서버)
    /// </summary>
    public static DateTime LastSyncedTimeLocal { get; private set; } = DateTime.MinValue;
    public static DateTime LastSyncedTimeServer { get; private set; } = DateTime.MinValue;
    public static bool IsSynced { get; private set; }
    public static bool IsMillisecondSynced { get; private set; }
    private static bool isSyncing;

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

            return DateTime.Now.AddTicks(CorrectionTick);
        }
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

        if (isSyncing || webRequest != null)
        {
            Debug.LogWarning("이미 동기화 시도 중입니다.");
            return;
        }

        isSyncing = true;

        var requestTime = DateTime.Now.Ticks;
        webRequest = UnityWebRequest.Get(setting.serverURI);

        webRequest.SendWebRequest().completed += operation =>
        {
            if (webRequest.error != null)
            {
                Error(webRequest.error);
                isSyncing = false;
            }
            else
            {
                if (DateTime.TryParse(webRequest.GetResponseHeader("date"), out var result))
                {
                    var responseTime = DateTime.Now;
                    CorrectionTick = result.ToLocalTime().Ticks - requestTime;

                    if (!IsMillisecondSynced && LastSyncedTimeLocal != DateTime.MinValue)
                    {
                        var localDiff = responseTime.Ticks - LastSyncedTimeLocal.Ticks;
                        var serverDiff = responseTime.AddTicks(CorrectionTick).Ticks - LastSyncedTimeServer.Ticks;
                        CorrectionTick += new DateTime(localDiff - serverDiff).Ticks;
                        IsMillisecondSynced = true;
                        Debug.Log("밀리초 동기화 완료");
                    }

                    LastSyncedTimeLocal = responseTime;
                    LastSyncedTimeServer = responseTime.AddTicks(CorrectionTick);

                    IsSynced = true;
                    isSyncing = false;
                    Debug.Log(Now);
                }
                else
                {
                    Error("헤더를 찾을 수 없음");
                    isSyncing = false;
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