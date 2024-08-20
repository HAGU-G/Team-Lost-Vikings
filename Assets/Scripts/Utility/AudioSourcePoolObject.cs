using UnityEngine;
using UnityEngine.Pool;

[RequireComponent (typeof(AudioSource))]
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
        {
            pool.Release(source);
        }
    }
}