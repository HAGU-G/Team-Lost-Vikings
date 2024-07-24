using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSelectorTest : MonoBehaviour, IPointerDownHandler
{
    public UnitSpawnTester spawner;

    public void OnPointerDown(PointerEventData eventData)
    {
        //spawner.Select(GetComponent<IStatUsable>());
    }
}