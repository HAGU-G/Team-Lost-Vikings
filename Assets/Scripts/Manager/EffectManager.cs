using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// 레이어 이름을 사용하기 때문에 코드 규칙을 따르지 않음
/// </summary>
public enum SORT_LAYER
{
    NONE,
    Grid,
    SkillFloor,
    RayReciver,
    Default,
    OverUnit,
    UI,
    OverUI
}


public class EffectManager : MonoBehaviour
{
    private Dictionary<string, IObjectPool<EffectObject>> effectPools = new();
    private Dictionary<string, AsyncOperationHandle> handles = new();

    public DamageEffect damageEffectPrefab;
    private IObjectPool<DamageEffect> damagePool;

    private InputManager im = null;


    private void Awake()
    {
        GameManager.Subscribe(EVENT_TYPE.LOADED, OnGameLoaded);
        GameManager.Subscribe(EVENT_TYPE.CONFIGURE, OnGameConfigure);
    }

    private void Update()
    {
        if (!GameManager.IsReady)
            return;

        if (im.Press)
        {
            var touchEffect = GetEffect(GameSetting.Instance.touchEffectName, SORT_LAYER.OverUI);
            touchEffect.transform.position = new(im.WorldPos.x, im.WorldPos.y, 0f);
            touchEffect.isTouchEffect = true;
        }
    }

    private void OnGameLoaded()
    {
        if (GameManager.effectManager != null)
        {
            Destroy(gameObject);
            return;
        }
        GameManager.effectManager = this;
    }

    private void OnGameConfigure()
    {
        im = GameManager.inputManager;

        // 데미지 이펙트 오브젝트 풀링
        damagePool = new ObjectPool<DamageEffect>(
            () =>
            {
                var damageEffect = Instantiate(damageEffectPrefab, transform);
                damageEffect.pool = damagePool;
                return damageEffect;
            },
            (x) =>
            {
                x.gameObject.SetActive(true);
            },
            null,
            null,
            true, 30, 10000
            );

        for (int i = 0; i < 20; i++)
        {
            damagePool.Get();
        }

        // 사용하는 유닛의 이펙트, 등장할 몬스터의 이펙트들 미리 생성
        List<string> effectNames = new();
        effectNames.Add(GameSetting.Instance.touchEffectName);

        foreach (var unit in GameManager.unitManager.Units.Values)
        {
            foreach (var skill in unit.Skills)
            {
                if (!effectNames.Contains(skill.Data.SkillEffectName))
                    effectNames.Add(skill.Data.SkillEffectName);

                if (!effectNames.Contains(skill.Data.ProjectileFileName))
                    effectNames.Add(skill.Data.ProjectileFileName);
            }
        }

        foreach (var effectName in effectNames)
        {
            AddPool(effectName);
        }
    }

    public DamageEffect GetDamageEffect(string text, Vector3 position, Color color, DamageEffect.TYPE type = DamageEffect.TYPE.DEFAULT)
    {
        var damageEffect = damagePool.Get();
        damageEffect.Set(text, position, color, type);

        return damageEffect;
    }
    public EffectObject GetEffect(string effectName, SORT_LAYER layer = SORT_LAYER.NONE)
    {
        if (!effectPools.ContainsKey(effectName))
        {
            if (!AddPool(effectName))
                return null;
        }

        var effect = effectPools[effectName].Get();
        if (layer != SORT_LAYER.NONE)
            effect.sortingGroup.sortingLayerName = layer.ToString();
        return effect;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="effectName"></param>
    /// <returns>어드레서블에 이펙트가 없을 경우 false 반환</returns>
    private bool AddPool(string effectName)
    {
        if (effectPools.ContainsKey(effectName))
            return true;

        if (Addressables.LoadResourceLocationsAsync(effectName).WaitForCompletion().Count <= 0)
            return false;

        var handle = Addressables.LoadAssetAsync<GameObject>(effectName);
        var go = handle.WaitForCompletion();
        var effect = go.GetComponent<EffectObject>();
        if (effect == null)
            effect = go.AddComponent<EffectObject>();

        IObjectPool<EffectObject> cachedPool = null;
        IObjectPool<EffectObject> pool = new LinkedPool<EffectObject>(
            () =>
            {
                var poolObject = Instantiate(effect, transform);
                poolObject.pool = cachedPool;
                return poolObject;
            },
            OnGetEffect,
            OnReleaseEffect,
            OnDestroyEffect, true, 30);

        cachedPool = pool;

        effectPools.Add(effectName, pool);
        handles.Add(effectName, handle);

        return true;
    }

    private void RemovePool(string effectName)
    {
        if (!effectPools.ContainsKey(effectName))
            return;

        effectPools.Remove(effectName);
        Addressables.Release(handles[effectName]);
        handles.Remove(effectName);
    }

    private void OnGetEffect(EffectObject effectObject)
    {
        effectObject.gameObject.SetActive(true);
    }

    private void OnReleaseEffect(EffectObject effectObject)
    {
        effectObject.transform.localScale = Vector3.one;
        effectObject.isOnProjectile = false;
        effectObject.transform.rotation = Quaternion.identity;
        effectObject.isTouchEffect = false;
    }

    private void OnDestroyEffect(EffectObject effectObject) { }
}