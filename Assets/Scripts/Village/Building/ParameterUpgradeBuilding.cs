using UnityEngine;

public class ParameterUpgradeBuilding : MonoBehaviour, IInteractableWithPlayer
{
    private PlayerManager pm;
    public STAT_TYPE upgradeParameter;
    public int upgradeValue;


    public void InteractWithPlayer()
    {
    }

    private void Start()
    {
        GameManager.Subscribe(EVENT_TYPE.START, OnGameStart);
        pm = GameManager.playerManager;
    }

    private void OnGameStart()
    {
        var buildingUp = GetComponent<BuildingUpgrade>();
        buildingUp.SetBuildingUpgrade();
        upgradeValue = buildingUp.StatReturn;
        RiseParameter();
    }

    public void RiseParameter()
    {
        switch(upgradeParameter)
        {
            case STAT_TYPE.HP:
                pm.unitHp.defaultValue = upgradeValue;
                break;
            case STAT_TYPE.STAMINA:
                pm.unitStamina.defaultValue = upgradeValue;
                break;
            case STAT_TYPE.MENTAL:
                pm.unitMental.defaultValue = upgradeValue;
                break;
        }
    }
}
