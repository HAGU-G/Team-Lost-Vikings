using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Recipe
{
    RECIPE1, //임시로 넣은 것임
    RECIPE2,
    RECIPE3,
}
public class ItemProduceBuilding : MonoBehaviour, IInteractableWithPlayer
{
    public Building building;
    public Recipe recipeId;
    public int itemStack;
    //제작 아이템 보관이라는 필드는 뭐지??

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
