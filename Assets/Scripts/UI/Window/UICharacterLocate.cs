using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterLocate : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.CHARACTER_LOCATE;

    private UnitStats unit;
    public Button village;
    public Button hpRecovery;
    public Button staminaRecovery;
    public Button stressRecovery;

    public Button exit;

    public Button[] huntzones;

    public GameObject locationPrefab;
    public Transform content;


    protected override void OnGameStart()
    {
        base.OnGameStart();

        var huntzones = GameManager.huntZoneManager.HuntZones;
        for(int i = 0; i < huntzones.Count; ++i)
        {
            int huntzoneNum = i;
            var location = Instantiate(locationPrefab, content);
            var locationComponent = location.GetComponent<Location>();
            locationComponent.locationName.text = $"{huntzoneNum + 1}번 사냥터";
            locationComponent.button.onClick.AddListener(() =>
            {
                SetUnitHuntZone(GameManager.huntZoneManager.HuntZones[huntzoneNum + 1].Info.HuntZoneNum);
            });
        }

        //for (int i = 0; i < huntzones.Length; ++i)
        //{
        //    int huntzoneNum = i;
        //    huntzones[i].onClick.AddListener(() =>
        //    {
        //        SetUnitHuntZone(GameManager.huntZoneManager.HuntZones[huntzoneNum + 1].Info.HuntZoneNum);
        //    });
        //}
    }

    private void OnEnable()
    {
        if (!IsReady)
            return;

        unit = GameManager.uiManager.currentUnitStats;
        
    }

    public void OnButtonExit()
    {
        Close();
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