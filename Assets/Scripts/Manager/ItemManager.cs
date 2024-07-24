using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ItemManager
{
    private int _gold;
    public int Gold 
    {
        get 
        {
            return _gold;
        }
        set 
        {
            _gold = value;
            GameManager.uiManager.uiDevelop.SetGold(value);
        } 
    }
    public int Rune { get; set; }

    public Dictionary<int, int> ownItemList = new();



    private void SetItemList()
    {
        ownItemList[1] = 0;
        ownItemList[2] = 0;
        ownItemList[3] = 0;
        ownItemList[4] = 0;
        ownItemList[5] = 0;
    }
}
