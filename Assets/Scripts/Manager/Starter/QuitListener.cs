using UnityEngine;

public class QuitListener : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        SaveManager.SaveGame();
    }
    private void OnApplicationPause(bool pause)
    {
        SaveManager.SaveGame();
    }
}
