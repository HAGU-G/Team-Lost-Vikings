using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSellBuilding : MonoBehaviour, IInteractableWithPlayer
{
    public Building building;
    public Recipe recipeId;

    private void Awake()
    {
        building = GetComponent<Building>();
    }

    public void InteractWithPlayer()
    {
        
    }

    private void SellItem()
    {

    }

    private void UnlockSellingRecipe()
    {

    }
}
