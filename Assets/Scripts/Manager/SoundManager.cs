using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Pool;
using UnityEngine.EventSystems;

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

    public IObjectPool<AudioSource> uiSource;

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

    public AudioSource locationSourcePrefab;
    private Dictionary<int, IObjectPool<AudioSource>> locationPool = new();
    private Dictionary<int, List<AudioSource>> locationSources = new();
    private LOCATION location = LOCATION.VILLAGE;
    private int currentHuntZoneNum = -1;
    private Dictionary<string, AsyncOperationHandle<AudioClip>> clipHandles = new();




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
        //UI 오디오 소스 추가
        uiSource = new LinkedPool<AudioSource>(
            () =>
            {
                var source = Instantiate(locationSourcePrefab, transform);
                source.name = $"UI";
                source.GetComponent<AudioSourcePoolObject>().pool = uiSource;
                AddSource(source, LOCATION.NONE);
                return source;
            },
            (x) => x.gameObject.SetActive(true),
            (x) => x.gameObject.SetActive(false),
            null,
            true, 1000);

        //마을 오디오 소스 추가
        IObjectPool<AudioSource> tempVPool = null;
        IObjectPool<AudioSource> villagePool = new LinkedPool<AudioSource>(
            () =>
            {
                var villageSource = Instantiate(locationSourcePrefab, transform);
                villageSource.name = $"Village";
                villageSource.GetComponent<AudioSourcePoolObject>().pool = tempVPool;
                AddSource(villageSource, LOCATION.VILLAGE);
                return villageSource;
            },
            (x) => x.gameObject.SetActive(true),
            (x) => x.gameObject.SetActive(false),
            null,
            true, 1000);
        tempVPool = villagePool;
        locationPool.Add(-1, villagePool);

        //사냥터 오디오 소스 추가
        foreach (var huntZone in GameManager.huntZoneManager.HuntZones)
        {
            IObjectPool<AudioSource> tempHPool = null;
            IObjectPool<AudioSource> huntZonePool = new LinkedPool<AudioSource>(
                () =>
                {
                    var huntZoneSource = Instantiate(locationSourcePrefab, transform);
                    huntZoneSource.name = $"HuntZone {huntZone.Key}";
                    huntZoneSource.GetComponent<AudioSourcePoolObject>().pool = tempHPool;
                    AddSource(huntZoneSource, LOCATION.HUNTZONE, huntZone.Key);
                    return huntZoneSource;
                },
                (x) => x.gameObject.SetActive(true),
                (x) => x.gameObject.SetActive(false),
                null,
                true, 1000);
            tempHPool = huntZonePool;
            locationPool.Add(huntZone.Key, huntZonePool);
        }

        //위치 설정
        SetLocation(LOCATION.VILLAGE);

        //GameManager.Subscribe(EVENT_TYPE.CONSTRUCT, SetAudioPlay);
    }

    private void Update()
    {
        //배경음악 반복 재생
        if (!bgmSource.isPlaying && Application.isFocused)
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

        switch (location)
        {
            case LOCATION.NONE:
                sm.uiSource.Get().PlayOneShot(clip, volumeScale);
                break;
            case LOCATION.VILLAGE:
                sm.locationPool[-1].Get().PlayOneShot(clip, volumeScale);
                break;
            case LOCATION.HUNTZONE:
                if (sm.locationPool.ContainsKey(huntZoneNum))
                {
                    sm.locationPool[huntZoneNum].Get().PlayOneShot(clip, volumeScale);
                }
                else
                {
                    sm.uiSource.Get().PlayOneShot(clip, volumeScale);
                    Debug.LogWarning($"사냥터 {huntZoneNum}번이 존재하지 않습니다. UI사운드로 재생합니다.");
                }
                break;
        }
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
            if (!(clipName == string.Empty || clipName == "0"))
                Debug.LogWarning($"{clipName} 사운드가 존재하지 않습니다.");
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
                        foreach (var source in sources.Value)
                        {
                            source.mute = false;
                        }
                    }
                    else
                    {
                        foreach (var source in sources.Value)
                        {
                            source.mute = true;
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
                        foreach (var source in sources.Value)
                        {
                            source.mute = false;
                        }
                    }
                    else
                    {
                        foreach (var source in sources.Value)
                        {
                            source.mute = true;
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

    public void AddSource(AudioSource source, LOCATION location, int huntZoneNum = -1)
    {
        switch (location)
        {
            case LOCATION.NONE:
                if (locationSources.TryGetValue(-2, out var uiList))
                {
                    if (!uiList.Contains(source))
                        uiList.Add(source);
                }
                else
                {
                    locationSources.Add(-2, new() { source });
                }
                break;
            case LOCATION.VILLAGE:
                source.mute = this.location != location;
                if (locationSources.TryGetValue(-1, out var villageList))
                {
                    if (!villageList.Contains(source))
                        villageList.Add(source);
                }
                else
                {
                    locationSources.Add(-1, new() { source });
                }
                break;
            case LOCATION.HUNTZONE:
                source.mute = !(this.location == location && currentHuntZoneNum == huntZoneNum);
                if (locationSources.TryGetValue(huntZoneNum, out var hunZoneList))
                {
                    if (!hunZoneList.Contains(source))
                        hunZoneList.Add(source);
                }
                else
                {
                    locationSources.Add(huntZoneNum, new() { source });
                }
                break;
        }
    }

    public void PlayButtonSFX() => PlaySFX(audioClickButton);
    public void PlayUnitSFX() => PlaySFX(audioClickUnit);
    public void PlayBuildingSFX() => PlaySFX(audioClickBuilding);
}
