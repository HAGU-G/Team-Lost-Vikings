using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public AudioMixer mixer;
    public AudioMixerGroup groupMusic;
    public AudioMixerGroup groupSfx;

    public AudioSource musicSource;
    public AudioClip[] musicCilps;
    private int currentMusicClip = 0;

    private void Awake()
    {
        if (GameManager.soundManager != null)
        {
            Destroy(gameObject);
            return;
        }

        GameManager.soundManager = this;

        musicSource.outputAudioMixerGroup = groupMusic;

        GameManager.Subscribe(EVENT_TYPE.START, OnGameStart);
    }

    private void OnGameStart()
    {
        GameManager.Subscribe(EVENT_TYPE.CONSTRUCT, SetAudioPlay);
    }

    private void Update()
    {
        if (!musicSource.isPlaying && Application.isFocused)
        {
            PlayBGM();
        }
    }

    private void PlayBGM()
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

}
