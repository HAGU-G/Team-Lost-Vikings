using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitListener : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        SaveManager.SaveGame();
    }
}
