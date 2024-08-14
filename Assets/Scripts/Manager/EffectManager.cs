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
            GetEffect(GameSetting.Instance.touchEffectName, SORT_LAYER.OverUI).transform.position =
                new(im.WorldPos.x, im.WorldPos.y, 0f);
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

    public EffectObject GetEffect(string effectName, SORT_LAYER layer = SORT_LAYER.NONE)
    {
        if (!effectPools.ContainsKey(effectName))
            AddPool(effectName);

        var effect = effectPools[effectName].Get();
        if (layer != SORT_LAYER.NONE)
            effect.sortingGroup.sortingLayerName = layer.ToString();
        return effect;
    }


    // 오브젝트 풀
    private void AddPool(string effectName)
    {
        if (effectPools.ContainsKey(effectName))
            return;

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

    private void OnReleaseEffect(EffectObject effectObject) { }

    private void OnDestroyEffect(EffectObject effectObject) { }
}