using UnityEngine;

public class StorageBuilding : MonoBehaviour, IInteractableWithPlayer
{
    private int _defaultGoldLimit = 4000;
    public int DefaultGoldLimit
    {
        get { return _defaultGoldLimit; }
        set { _defaultGoldLimit = value; }
    }

    public void InteractWithPlayer()
    {
        
    }

    public void UpgradeGoldLimit(int gold)
    {
        GameManager.itemManager.itemLimit = gold;
    }
}
