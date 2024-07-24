using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class UIUnitsInformation : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.UNITS_INFORMATION;

    public Button exit;


    public Transform content;
    public GameObject unitInfo;
    public List<GameObject> infos;


    private void OnEnable()
    {
        if (!IsReady)
            return;

        SetInfo();
    }

    public void SetInfo()
    {
        for(int i = 0; i < infos.Count; ++i)
        {
            Destroy(infos[i]);
        }
        infos.Clear();

        var units = GameManager.unitManager.Units;

        foreach(var unit in units)
        {
            var button = GameObject.Instantiate(unitInfo, content);
            var info = button.GetComponent<CharacterInfo>();
            info.characterName.text = $"{unit.Value.Name}";
            info.characterGrade.text = $"{unit.Value.UnitGrade}";
            info.characterIcon.uvRect
                = GameManager.uiManager.unitRenderTexture.LoadRenderTexture(unit.Value.AssetFileName);
            info.information.onClick.AddListener(
            () =>
            {
                GameManager.uiManager.currentUnitStats = unit.Value;
                GameManager.uiManager.windows[WINDOW_NAME.UNIT_DETAIL_INFORMATION].Open();
            }
                );

            infos.Add(button);
        }

        //for (int i = 0; i < units.Count; ++i)
        //{
        //    var button = GameObject.Instantiate(unitInfo, content);
        //    var unit = button.GetComponent<CharacterInfo>();
        //    unit.characterName.text = $"{units.GetValueOrDefault(i).Name}";
        //    unit.characterGrade.text = $"{units.GetValueOrDefault(i).UnitGrade}";
            
        //    unit.information.onClick.AddListener(
        //    () =>
        //    {
        //        GameManager.uiManager.currentUnitStats = units[i];
        //        GameManager.uiManager.windows[(int)WINDOW_NAME.UNIT_DETAIL_INFORMATION].Open();
        //    }
        //        );
        //}
    }


    public void OnButtonExit()
    {
        Close();
    }
}
