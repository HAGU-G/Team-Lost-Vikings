using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public int Gold { get; set; }
    public int Rune { get; set; }


    private void Awake()
    {
        if (GameManager.itemManager != null)
        {
            Destroy(gameObject);
            return;
        }

        GameManager.itemManager = this;
    }
}
