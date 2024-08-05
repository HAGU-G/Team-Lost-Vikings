using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    private int currentClip = 0;

    private void Awake()
    {
        if (GameManager.soundManager != null)
        {
            Destroy(gameObject);
            return;
        }

        GameManager.soundManager = this;

        GameManager.Subscribe(EVENT_TYPE.START, OnGameStart);
    }

    private void OnGameStart()
    {
        GameManager.Subscribe(EVENT_TYPE.CONSTRUCT, SetAudioPlay);
    }

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlayBGM();
        }
    }

    private void PlayBGM()
    {
        currentClip = currentClip % audioClips.Length;
        audioSource.clip = audioClips[currentClip];
        audioSource.Play();
        ++currentClip;
    }

    private void SetAudioPlay()
    {
        if(GameManager.villageManager.constructMode.currentTimeScale == 0f)
        {
            audioSource.pitch = 1f;
        }
    }

}
