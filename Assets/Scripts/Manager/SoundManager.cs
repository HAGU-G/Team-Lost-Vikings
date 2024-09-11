using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Pool;

public class SoundManager : MonoBehaviour
{
    [Header("오디오 믹서")]
    public AudioMixer mixer;
    public AudioMixerGroup bgmGroup;
    public AudioMixerGroup sfxGroup;
    public static readonly string paramMasterVolume = "Master";
    public static readonly string paramBGMVolume = "Bgm";
    public static readonly string paramSFXVolume = "Sfx";

    [Header("배경음악")]
    public AudioSource bgmSource;
    public AudioClip[] bgmCilps;
    private int currentBGMClip = 0;

    [Header("효과음")]
    public AudioClip audioClickUnit;
    public AudioClip audioClickButton;
    public AudioClip audioClickBuilding;

    private float _masterVolume = 1f;
    private float _bgmVolume = 1f;
    private float _sfxVolume = 1f;

    public static float MasterVolume
    {
        get
        {
            var sm = GameManager.soundManager;
            if (sm == null)
                return 0f;

            return sm._masterVolume;
        }
        set
        {
            var sm = GameManager.soundManager;
            if (sm == null)
                return;

            sm.mixer.SetFloat(paramMasterVolume, FloatToDb(value));
            sm._masterVolume = Mathf.Clamp(value, 0f, 1f);
        }
    }
    public static float BGMVolume
    {
        get
        {
            var sm = GameManager.soundManager;
            if (sm == null)
                return 0f;

            return sm._bgmVolume;
        }
        set
        {
            var sm = GameManager.soundManager;
            if (sm == null)
                return;

            sm.mixer.SetFloat(paramBGMVolume, FloatToDb(value));
            sm._bgmVolume = Mathf.Clamp(value, 0f, 1f);
        }
    }
    public static float SFXVolume
    {
        get
        {
            var sm = GameManager.soundManager;
            if (sm == null)
                return 0f;

            return sm._sfxVolume;
        }
        set
        {
            var sm = GameManager.soundManager;
            if (sm == null)
                return;

            sm.mixer.SetFloat(paramSFXVolume, FloatToDb(value));
            sm._sfxVolume = Mathf.Clamp(value, 0f, 1f);
        }
    }

    public AudioSourcePoolObject locationSourcePrefab;

    private IObjectPool<AudioSourcePoolObject> audioPool;
    private Dictionary<int, LinkedList<AudioSourcePoolObject>> locationSources = new();
    private LinkedList<AudioSourcePoolObject> playingSources = new();
    private Dictionary<string, AsyncOperationHandle<AudioClip>> clipHandles = new();
    private int activeSourceCount = 0;

    private LOCATION location = LOCATION.VILLAGE;
    private int currentHuntZoneNum = -1;





    private void Awake()
    {
        if (GameManager.soundManager != null)
        {
            Destroy(gameObject);
            return;
        }

        GameManager.soundManager = this;

        bgmSource.outputAudioMixerGroup = bgmGroup;

        GameManager.Subscribe(EVENT_TYPE.START, OnGameStart);
    }

    private void OnGameStart()
    {
        //마을 오디오 소스 추가
        IObjectPool<AudioSourcePoolObject> tempPool = null;
        audioPool = new LinkedPool<AudioSourcePoolObject>(
            () =>
            {
                var audioSource = Instantiate(locationSourcePrefab, transform);
                audioSource.pool = tempPool;
                return audioSource;
            },
            (get) =>
            {
                get.gameObject.SetActive(true);
                activeSourceCount++;
            },
            (release) =>
            {
                RemoveLocationSource(release);
                release.gameObject.SetActive(false);
                activeSourceCount--;
            },
            (destroy) =>
            {
                RemoveLocationSource(destroy);
                destroy.Stop();
                activeSourceCount--;
            },
            true, 31);
        tempPool = audioPool;

        List<AudioSourcePoolObject> sources = new();
        for (int i = 0; i < 8; i++)
        {
            audioPool.Get();
        }
        foreach (var source in sources)
        {
            source.Release();
        }

        //위치 설정
        SetLocation(LOCATION.VILLAGE);
    }

    private void RemoveLocationSource(AudioSourcePoolObject audioSource)
    {
        foreach (var list in locationSources.Values)
        {
            if (list.Remove(audioSource))
            {
                playingSources.Remove(audioSource);
                break;
            }
        }
    }

    private void Update()
    {
        //배경음악 반복 재생
        if (GameManager.IsReady && !bgmSource.isPlaying && Application.isFocused)
            AutoPlayBGM();
    }

    private static void PlayBGM(AudioClip clip)
    {
        var sm = GameManager.soundManager;
        if (sm == null)
            return;

        sm.bgmSource.PlayOneShot(clip);
    }

    private void AutoPlayBGM()
    {
        currentBGMClip = currentBGMClip % bgmCilps.Length;
        bgmSource.PlayOneShot(bgmCilps[currentBGMClip]);
        ++currentBGMClip;
    }

    private void SetAudioPlay()
    {
        if (GameManager.villageManager.constructMode.currentTimeScale == 0f)
        {
            bgmSource.pitch = 1f;
        }
    }

    /// <summary>
    /// 오디오 클립으로 효과음 재생
    /// </summary>
    /// <param name="clip">오디오 클립</param>
    /// <param name="location">재생할 지역, NONE일 경우 UI사운드로 취급</param>
    /// <param name="huntZoneNum">사냥터 번호</param>
    public static void PlaySFX(AudioClip clip, LOCATION location = LOCATION.NONE, int huntZoneNum = -1, float volumeScale = 1f)
    {
        var sm = GameManager.soundManager;
        if (sm == null)
            return;

        if (clip == null)
            return;


        AudioSourcePoolObject audioSource = null;
        if (sm.activeSourceCount >= 31)
        {
            if (location == sm.location && sm.playingSources.Count > 0)
            {
                sm.playingSources.First.Value.Release();
                audioSource = sm.audioPool.Get();
            }
            else
            {
                return; 
            }
        }
        else
        {
            audioSource = sm.audioPool.Get();
        }

        if (audioSource == null)
            return;

        sm.AddSource(audioSource, location, huntZoneNum);
        audioSource.PlayOneShot(clip, volumeScale);
    }

    /// <summary>
    /// 오디오 클립 이름으로 효과음 재생
    /// </summary>
    /// <param name="clipName">오디오 클립 이름, 어드레서블에 등록되어있어야 함.</param>
    /// <param name="location">재생할 지역, NONE일 경우 UI사운드로 취급</param>
    /// <param name="huntZoneNum">사냥터 번호</param>
    public static void PlaySFX(string clipName, LOCATION location = LOCATION.NONE, int huntZoneNum = -1, float volumeScale = 1f)
    {
        var sm = GameManager.soundManager;
        if (sm == null)
            return;

        AudioClip clip = null;

        if (sm.clipHandles.ContainsKey(clipName))
        {
            clip = sm.clipHandles[clipName].Result;
        }
        else if (Addressables.LoadResourceLocationsAsync(clipName).WaitForCompletion().Count <= 0)
        {
            //if (!(clipName == string.Empty || clipName == "0"))
            //    Debug.LogWarning($"{clipName} 사운드가 존재하지 않습니다.");
        }
        else
        {
            var handle = Addressables.LoadAssetAsync<AudioClip>(clipName);
            clip = handle.WaitForCompletion();
            sm.clipHandles.Add(clipName, handle);
        }

        PlaySFX(clip, location, huntZoneNum, volumeScale);
    }

    public void ReleaseClip(string clipName)
    {
        if (clipHandles.TryGetValue(clipName, out var handle))
        {
            clipHandles.Remove(clipName);
            Addressables.Release(handle);
        }
    }

    private void OnDestroy()
    {
        List<string> clipNames = new();
        foreach (var clipName in clipHandles.Keys)
        {
            clipNames.Add(clipName);
        }

        foreach (var clipName in clipNames)
        {
            ReleaseClip(clipName);
            clipHandles.Remove(clipName);
        }
    }

    public void SetLocation(LOCATION location, int huntZoneNum = -1)
    {
        switch (location)
        {
            case LOCATION.NONE:
                break;
            case LOCATION.VILLAGE:
                this.location = location;
                foreach (var sources in locationSources)
                {
                    if (sources.Key == -2)
                        continue;

                    if (sources.Key == -1)
                    {
                        foreach (var sourceObject in sources.Value)
                        {
                            sourceObject.source.mute = false;
                        }
                    }
                    else
                    {
                        foreach (var sourceObject in sources.Value)
                        {
                            sourceObject.source.mute = true;
                        }
                    }
                }
                break;
            case LOCATION.HUNTZONE:
                this.location = location;
                currentHuntZoneNum = huntZoneNum;
                foreach (var sources in locationSources)
                {
                    if (sources.Key == -2)
                        continue;

                    if (sources.Key == huntZoneNum)
                    {
                        foreach (var sourceObject in sources.Value)
                        {
                            sourceObject.source.mute = false;
                        }
                    }
                    else
                    {
                        foreach (var sourceObject in sources.Value)
                        {
                            sourceObject.source.mute = true;
                        }
                    }
                }
                break;
        }
    }

    public static float FloatToDb(float value)
    {
        if (value == 0f)
            return -144.0f;
        else
            return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
    }

    public void AddSource(AudioSourcePoolObject sourceObject, LOCATION location, int huntZoneNum = -1)
    {
        playingSources.AddLast(sourceObject);
        switch (location)
        {
            case LOCATION.NONE:
                sourceObject.source.mute = false;
                if (locationSources.TryGetValue(-2, out var uiList))
                {
                    if (!uiList.Contains(sourceObject))
                        uiList.AddLast(sourceObject);
                }
                else
                {
                    locationSources.Add(-2, new());
                    locationSources[-2].AddLast(sourceObject);
                }
                break;
            case LOCATION.VILLAGE:
                sourceObject.source.mute = this.location != location;
                if (locationSources.TryGetValue(-1, out var villageList))
                {
                    if (!villageList.Contains(sourceObject))
                        villageList.AddLast(sourceObject);
                }
                else
                {
                    locationSources.Add(-1, new());
                    locationSources[-1].AddLast(sourceObject);
                }
                break;
            case LOCATION.HUNTZONE:
                sourceObject.source.mute = !(this.location == location && currentHuntZoneNum == huntZoneNum);
                if (locationSources.TryGetValue(huntZoneNum, out var hunZoneList))
                {
                    if (!hunZoneList.Contains(sourceObject))
                        hunZoneList.AddLast(sourceObject);
                }
                else
                {
                    locationSources.Add(huntZoneNum, new());
                    locationSources[huntZoneNum].AddLast(sourceObject);
                }
                break;
        }
    }

    public void PlayButtonSFX() => PlaySFX(audioClickButton);
    public void PlayUnitSFX() => PlaySFX(audioClickUnit);
    public void PlayBuildingSFX() => PlaySFX(audioClickBuilding);
}
