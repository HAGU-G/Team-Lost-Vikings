using UnityEngine;

public class QuitListener : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        if (GameManager.IsReady)
            SaveManager.SaveGame();
    }
    private void OnApplicationPause(bool pause)
    {
        if (GameManager.IsReady)
            SaveManager.SaveGame();
    }
}
