using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

public class UICharacterLocate : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.CHARACTER_LOCATE;

    private UnitStats unit;
    public Button village;
    public Button hpRecovery;
    public Button staminaRecovery;
    public Button stressRecovery;

    public Button[] huntzones;

    private void Start()
    {
        for(int i = 0; i < huntzones.Length; ++i)
        {
            huntzones[i].onClick.AddListener(() =>
            {
                SetUnitHuntZone(i + 1);
            });
        }
    }

    private void OnEnable()
    {
        unit = GameManager.uiManager.currentUnitStats;
        
    }

    public void OnButtonHp()
    {
        
    }

    public void SetUnitRecoveryBuilding(int recovery)
    {

    }

    public void SetUnitHuntZone(int huntzone)
    {
        unit.SetHuntZone(huntzone);
    }
}