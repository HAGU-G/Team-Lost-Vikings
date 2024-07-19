using UnityEngine;

public class ItemProduceBuilding : MonoBehaviour, IInteractableWithPlayer
{
    public Building building;
    public RECIPE recipeId;
    public int itemStack;
    

    private void Awake()
    {
        building = GetComponent<Building>();
    }

    public void InteractWithPlayer()
    {
        
    }

    private void ProduceItem()
    {

    }

    private void RestoreProducedItem()
    {

    }

    private void UnlockProduceRecipe()
    {

    }
}
