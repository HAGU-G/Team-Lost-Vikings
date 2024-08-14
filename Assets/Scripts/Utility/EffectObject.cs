using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;

public class EffectObjectReference : AssetReferenceT<EffectObject>
{
    public EffectObjectReference(string guid) : base(guid)
    {
    }
}

[RequireComponent(typeof(SortingGroup))]
public class EffectObject : MonoBehaviour
{
    private List<ParticleSystem> particleSystems = new();
    public SortingGroup sortingGroup;

    private bool isStopped = false;
    public bool isLoop = false;
    public bool isFlip = false;
    [HideInInspector] public bool isOnProjectile = false;
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
            p.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        isStopped = true;
    }

    private void Update()
    {
        if (!isLoop && !isOnProjectile)
        {
            bool isParticleStopped = true;
            foreach (var p in particleSystems)
            {
                isParticleStopped &= p.isStopped;
            }

            gameObject.SetActive(!isParticleStopped);
        }

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

    public void LookAt(Vector3 targetPos)
    {
        var delta = targetPos - transform.position;
        var rotationDeg = Mathf.Rad2Deg * Mathf.Atan2(delta.y, delta.x);
        transform.eulerAngles = new (0f, isFlip ? 180f : 0f, isFlip ? 180f - rotationDeg : rotationDeg);
    }
}
