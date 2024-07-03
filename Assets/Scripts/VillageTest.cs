using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageTest : MonoBehaviour
{
    public VillageManager villageManager;

    private GameObject building;
    public GameObject hospital;
    private bool isSelected;

    private void Start()
    {
        isSelected = false;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && isSelected)
        {
            var mousePos = Input.mousePosition;
            var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            worldPos.z = 0f;
            var build = Instantiate(building, worldPos, Quaternion.identity);
            var buildingComponent = building.GetComponent<Building>();
            villageManager.construectedBuildings.Add(buildingComponent);

            Debug.Log($"Name : {buildingComponent.Name}, Cost : {buildingComponent.Cost}, Type : {buildingComponent.Type}");
            isSelected = false;
            return;
        }

        if(Input.GetMouseButtonDown(0) && !isSelected)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 100f);

            if(hit.collider != null)
            {
                Building building = hit.transform.gameObject.GetComponent<Building>();
                if(building != null)
                {
                    building.Interact();
                }

            }
        }
    }

    private void OnGUI()
    {
        if(GUI.Button(new Rect(0f,0f,50f,50f), "Hospital"))
        {
            building = hospital;
            isSelected = true;
        }
    }
}
