using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HospitalInteract : IInteractableWithPlayer
{
    public void InteractWithPlayer()
    {
        Debug.Log("병원입니다.");
    }
}
