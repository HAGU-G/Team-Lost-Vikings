using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum BUILDING_TYPE
{
    HOSPITAL,
    WEAPON_STORE,
    ACCESSORY_STORE,
    RESTAURANT,
}

public class Building : MonoBehaviour
{
    [field:SerializeField]
    public string Name { get; set; }
    [field: SerializeField]
    public int Cost { get; private set; }
    [field: SerializeField]
    public BUILDING_TYPE Type { get; set; }

    private IInteractable interact;


    public Building(string name, int cost, BUILDING_TYPE type)
    {
        Name = name;
        Cost = cost;
        Type = type;

        switch(Type)
        {
            case BUILDING_TYPE.HOSPITAL:
                interact = new HospitalInteract();
                break;

        }
    }

    public void Interact()
    {
        interact?.Interact();
    }

    private void Update()
    {

    }
}
