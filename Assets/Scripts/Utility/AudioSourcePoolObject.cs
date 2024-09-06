using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(AudioSource))]
public class AudioSourcePoolObject : MonoBehaviour
{
    public IObjectPool<AudioSource> pool;
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!source.isPlaying)
            Release();
    }

    public void Release()
    {
        source.Stop();

        if (pool != null)
            pool.Release(source);
        else
            Destroy(source);
    }
}