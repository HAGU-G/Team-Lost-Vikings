using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SoundManager : MonoBehaviour
{
    [Header("오디오 믹서")]
    public AudioMixer mixer;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;

    [Header("배경음악")]
    public AudioSource musicSource;
    public AudioClip[] musicCilps;
    private int currentMusicClip = 0;

    [Header("효과음")]
    public AudioSource uiSource;

    public AudioSource locationSourcePrefab;
    private Dictionary<int, AudioSource> locationSources = new();

    private Dictionary<string, AsyncOperationHandle> clipHandles = new();

    private void Awake()
    {
        if (GameManager.soundManager != null)
        {
            Destroy(gameObject);
            return;
        }

        GameManager.soundManager = this;

        musicSource.outputAudioMixerGroup = musicGroup;

        GameManager.Subscribe(EVENT_TYPE.START, OnGameStart);
    }

    private void OnGameStart()
    {
        var villageSource = Instantiate(locationSourcePrefab, transform);
        villageSource.name = $"Village";
        locationSources.Add(-1, villageSource);

        foreach (var huntZone in GameManager.huntZoneManager.HuntZones)
        {
            var huntZoneSource = Instantiate(locationSourcePrefab, transform);
            huntZoneSource.name = $"HuntZone {huntZone.Key}";
            locationSources.Add(huntZone.Key, huntZoneSource);
        }

        //GameManager.Subscribe(EVENT_TYPE.CONSTRUCT, SetAudioPlay);
    }

    private void Update()
    {
        //배경음악 반복 재생
        if (!musicSource.isPlaying && Application.isFocused)
            AutoPlayBGM();
    }

    private static void PlayBGM(AudioClip clip)
    {
        var sm = GameManager.soundManager;
        if (sm == null)
            return;

        sm.musicSource.PlayOneShot(clip);
    }

    private void AutoPlayBGM()
    {
        currentMusicClip = currentMusicClip % musicCilps.Length;
        musicSource.PlayOneShot(musicCilps[currentMusicClip]);
        ++currentMusicClip;
    }

    private void SetAudioPlay()
    {
        if (GameManager.villageManager.constructMode.currentTimeScale == 0f)
        {
            musicSource.pitch = 1f;
        }
    }

    /// <summary>
    /// 오디오 클립으로 효과음 재생
    /// </summary>
    /// <param name="clip">오디오 클립</param>
    /// <param name="location">재생할 지역, NONE일 경우 UI사운드로 취급</param>
    /// <param name="huntZoneNum">사냥터 번호</param>
    public static void PlaySfx(AudioClip clip, LOCATION location = LOCATION.NONE, int huntZoneNum = 0)
    {
        var sm = GameManager.soundManager;
        if (sm == null)
            return;

        switch (location)
        {
            case LOCATION.NONE:
                sm.uiSource.PlayOneShot(clip);
                break;
            case LOCATION.VILLAGE:
                sm.locationSources[-1].PlayOneShot(clip);
                break;
            case LOCATION.HUNTZONE:
                if (sm.locationSources.ContainsKey(huntZoneNum))
                {
                    sm.locationSources[huntZoneNum].PlayOneShot(clip);
                }
                else
                {
                    sm.uiSource.PlayOneShot(clip);
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
    public static void PlaySfx(string clipName, LOCATION location = LOCATION.NONE, int huntZoneNum = 0)
    {
        var sm = GameManager.soundManager;
        if (sm == null)
            return;

        AudioClip clip = null;

        if (sm.clipHandles.ContainsKey(clipName))
        {
            clip = sm.clipHandles[clipName].Result as AudioClip;
        }
        else
        {
            var handle = Addressables.LoadAssetAsync<AudioClip>(clipName);
            clip = handle.WaitForCompletion();
            sm.clipHandles.Add(clipName, handle);
        }

        PlaySfx(clip, location, huntZoneNum);
    }

    public void ReleaseClip(string clipName)
    {
        if (clipHandles.TryGetValue(clipName, out var handle))
        {
            clipHandles.Remove(clipName);
            Addressables.Release(handle);
        }
    }
}
