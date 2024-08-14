using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(SortingGroup))]
public class EffectObject : MonoBehaviour
{
    private List<ParticleSystem> particleSystems = new();
    public SortingGroup sortingGroup;

    private bool isStopped = false;
    public IObjectPool<EffectObject> pool;

    private void Awake()
    {
        sortingGroup = GetComponent<SortingGroup>();
        sortingGroup.sortAtRoot = true;
        particleSystems = GetComponentsInChildren<ParticleSystem>().ToList();

        if (!GameManager.IsReady)
            Stop();
    }

    public void Stop()
    {
        foreach (var p in particleSystems)
        {
            p.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        isStopped = true;
    }

    private void Update()
    {
        if (!isStopped)
        {
            sortingGroup.sortingOrder = -Mathf.FloorToInt(gameObject.transform.position.y);
        }
        else
        {
            bool isParticleStopped = true;
            foreach (var p in particleSystems)
            {
                isParticleStopped &= p.isStopped;
            }

            gameObject.SetActive(!isParticleStopped);
        }
    }

    private void OnEnable()
    {
        isStopped = false;
        foreach (var p in particleSystems)
        {
            p.Play(true);
        }
    }

    private void OnDisable()
    {
        if (pool != null)
            pool.Release(this);
        else if (!Addressables.ReleaseInstance(gameObject))
            Destroy(gameObject);
    }
}
