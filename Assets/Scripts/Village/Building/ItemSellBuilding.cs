using UnityEngine;

public class ItemSellBuilding : MonoBehaviour, IInteractableWithPlayer
{
    public Building building;
    public RECIPE recipeId;

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
