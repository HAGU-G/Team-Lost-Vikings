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


    public void SetInfo()
    {
        for(int i = 0; i < infos.Count; ++i)
        {
            Destroy(infos[i]);
        }
        infos.Clear();

        var units = GameManager.unitManager.Waitings;

        for (int i = 0; i < units.Count; ++i)
        {
            var button = Instantiate(unitInfo, content);
            var unit = button.GetComponent<CharacterInfo>();
            unit.characterName.text = $"{units[i].Name}";
            unit.characterGrade.text = $"{units[i].UnitGrade}";
            
            unit.information.onClick.AddListener(
            () =>
            {
                GameManager.uiManager.currentUnitStats = units[i];
                GameManager.uiManager.windows[(int)WINDOW_NAME.UNIT_DETAIL_INFORMATION].Open();
            }
                );
        }
    }


    public void OnButtonExit()
    {
        Close();
    }
}
