using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecruitBuilding : MonoBehaviour, IInteractableWithPlayer
{
    private int _defaultGachaUnlockLevel = 0;
    public int DefaultGachaUnlockLevel
    {
        get { return _defaultGachaUnlockLevel; }
        set { _defaultGachaUnlockLevel = value; }
    }

    public void InteractWithPlayer()
    {
        
    }

    public void UpgradeUnlockLevel(int value)
    {
        GameManager.playerManager.recruitLevel = value;
    }
}
