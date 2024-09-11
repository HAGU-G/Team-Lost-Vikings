using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(AudioSource))]
public class AudioSourcePoolObject : MonoBehaviour
{
    public IObjectPool<AudioSourcePoolObject> pool;
    public AudioSource source;

    private void Update()
    {
        if (!source.isPlaying)
            Release();
    }

    public void Release()
    {
        source.Stop();

        if (pool != null)
            pool.Release(this);
        else
            Destroy(this);
    }

    public void PlayOneShot(AudioClip clip, float volumeScale) => source.PlayOneShot(clip, volumeScale);
    public void Stop() => source.Stop();
    public void Pause() => source.Pause();
    public void UnPause() => source.UnPause();

}