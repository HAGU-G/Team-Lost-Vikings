using System.Collections;
using System.Collections.Generic;
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

        GameManager.Subscribe(EVENT_TYPE.INIT, OnGameInit);
    }

    private void OnGameInit()
    {
        //audioSource.GetComponent<AudioSource>();

        audioSource.clip = audioClips[currentClip];
        audioSource.Play();
    }

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlayNextBGM();
        }
    }

    private void PlayNextBGM()
    {
        currentClip = (currentClip + 1) % audioClips.Length;
        audioSource.clip = audioClips[currentClip];
        audioSource.Play();
    }

}
