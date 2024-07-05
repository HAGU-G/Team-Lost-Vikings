using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum STRUCTURE_TYPE
{
    HOSPITAL,
    WEAPON_STORE,
    ACCESSORY_STORE,
    RESTAURANT,
}

public class Building : MonoBehaviour
{
    [field:SerializeField] //데이터 테이블에서 받아오기 전 임시로 입력
    public string Name { get; set; }
    [field: SerializeField]
    public int WidthSize { get; set; }
    [field: SerializeField]
    public int HeightSize { get; set; }
    [field: SerializeField]
    public STRUCTURE_TYPE Type { get; set; }
    [field: SerializeField]
    public bool CanMultiBuild { get; set; }
    [field: SerializeField]
    public int UnlockTownLevel { get; set; }
    [field: SerializeField]
    public int UpgradeId { get; set; }
    [field: SerializeField]
    public string AssetFileName { get; set; }


    private IInteractable interact;

    private void Awake()
    {


        switch (Type)
        {
            case STRUCTURE_TYPE.HOSPITAL:
                interact = new HospitalInteract();
                break;

        }
    }
    //public Building(string name, STRUCTURE_TYPE type)
    //{
    //    Name = name;
    //    Type = type;
    //}

    public void Interact()
    {
        interact?.Interact();
    }

    private void Update()
    {

    }
}
