using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 레이어 이름을 사용하기 때문에 코드 규칙을 따르지 않음
/// </summary>
public enum SORT_LAYER
{
    Grid,
    SkillFloor,
    RayReciver,
    Default,
    UI,
    OverUI
}


public class EffectManager : MonoBehaviour
{
    private Dictionary<string, IObjectPool<EffectObject>> effects;


    private void Awake()
    {
        GameManager.Subscribe(EVENT_TYPE.LOADED, OnGameLoaded);
        GameManager.Subscribe(EVENT_TYPE.CONFIGURE, OnGameConfigure);
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
        // 사용하는 유닛의 이펙트, 등장할 몬스터의 이펙트들 미리 생성
        
    }

    public EffectObject SpawnEffect(string effectName, SORT_LAYER layer)
    {
        var effect = effects[effectName].Get();
        effect.sortingGroup.sortingLayerName = layer.ToString();
        return effect;
    }
}