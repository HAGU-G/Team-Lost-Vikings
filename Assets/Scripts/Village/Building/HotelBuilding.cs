using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotelBuilding : MonoBehaviour, IInteractableWithPlayer
{
    private int _defaultUnitLimit = 4;
    public int DefaultUnitLimit
    {
        get { return _defaultUnitLimit; }
        set { _defaultUnitLimit = int.MaxValue; }
    }


    public void InteractWithPlayer()
    {

    }

    public void UpgradeUnitLimit(int amount)
    {
        
    }

}
