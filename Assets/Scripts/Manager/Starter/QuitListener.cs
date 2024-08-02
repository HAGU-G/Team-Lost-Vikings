using UnityEngine;

public class QuitListener : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        SaveManager.SaveGame();
    }
}
