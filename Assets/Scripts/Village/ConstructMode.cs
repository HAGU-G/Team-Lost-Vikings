using UnityEngine;

public class ConstructMode
{
    public Construct construct;
    public float currentTimeScale;

    public void Init()
    {
        GameManager.Subscribe(EVENT_TYPE.CONSTRUCT, SetConstructMode);
        currentTimeScale = 1f;
    }

    private void SetConstructMode()
    {
        Time.timeScale = (currentTimeScale == 1f) ? 0f : 1f;
    }



}
