using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;
using AssetKits.ParticleImage;

public class EffectObjectReference : AssetReferenceT<EffectObject>
{
    public EffectObjectReference(string guid) : base(guid)
    {
    }
}

[RequireComponent(typeof(SortingGroup))]
public class EffectObject : MonoBehaviour
{
    public List<ParticleImage> particleImages = new();
    private List<ParticleSystem> particleSystems = new();
    public List<Canvas> canvases = new();
    [HideInInspector] public SortingGroup sortingGroup;

    private bool isStopped = false;
    public SORT_LAYER defaultSortLayer = SORT_LAYER.Default;
    public bool isLoop = false;
    [HideInInspector] public bool isFlip = false;
    [HideInInspector] public bool isTouchEffect = false;
    [HideInInspector] public bool isOnProjectile = false;
    public IObjectPool<EffectObject> pool;
    public bool IsParticleStopped
    {
        get
        {
            bool isParticleStopped = true;
            foreach (var p in particleSystems)
            {
                isParticleStopped &= p.isStopped;
            }
            foreach (var p in particleImages)
            {
                isParticleStopped &= p.isStopped;
            }
            return isParticleStopped;
        }
    }
    public Vector3 prevPos;

    private void Awake()
    {
        sortingGroup = GetComponent<SortingGroup>();
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
        foreach (var p in particleImages)
        {
            p.Stop();
        }

        isStopped = true;
    }

    private void Update()
    {
        if (sortingGroup.sortingLayerName == SORT_LAYER.OverUI.ToString()
            || sortingGroup.sortingLayerName == SORT_LAYER.UI.ToString())
        {
            transform.localScale =
                Vector3.one
                * Camera.main.orthographicSize
                * (isTouchEffect ? GameSetting.Instance.touchEffectScale : 1f);

            transform.position += GameManager.cameraManager.DeltaPos;
        }
        else
        {
            sortingGroup.sortingOrder = -Mathf.FloorToInt(gameObject.transform.position.y);
        }
        if ((!isLoop && !isOnProjectile) || isStopped)
            gameObject.SetActive(!IsParticleStopped);
    }

    public void UseScaledDeltaTime(bool isScaled)
    {

        foreach (var p in particleSystems)
        {
            var main = p.main;
            main.useUnscaledTime = !isScaled;
        }
        foreach (var p in particleImages)
        {
            var timeScale = isScaled ? AssetKits.ParticleImage.Enumerations.TimeScale.Normal
                                     : AssetKits.ParticleImage.Enumerations.TimeScale.Unscaled;
            p.timeScale = timeScale;
            foreach (var piChild in p.children)
            {
                piChild.timeScale = timeScale;
            }
        }
    }

    private void LateUpdate()
    {
        if (prevPos != transform.position)
        {
            foreach (var p in particleImages)
            {
                p.transform.position = transform.position;
            }
        }

        prevPos = transform.position;
    }

    private void OnEnable()
    {
        isStopped = false;
        foreach (var p in particleSystems)
        {
            p.Play(true);
        }
        foreach (var p in particleImages)
        {
            p.Play();
        }
    }

    private void OnDisable()
    {
        if (pool != null)
            pool.Release(this);
        else if (!Addressables.ReleaseInstance(gameObject))
            Destroy(gameObject);
    }

    public void LookAt(Vector3 targetPos, bool isOnlyFlip = false)
    {
        var delta = targetPos - transform.position;
        if (isOnlyFlip)
        {
            delta.y = 0f;
            delta.z = 0f;
        }
        var rotationDeg = Mathf.Rad2Deg * Mathf.Atan2(delta.y, delta.x);
        float angleZ = isOnlyFlip ? transform.rotation.z : (isFlip ? 180f - rotationDeg : rotationDeg);
        transform.rotation = Quaternion.Euler(0f, isFlip ? 180f : 0f, angleZ);
    }
}
