using System.Collections.Generic;
using UnityEngine;

public class ConstructMode
{
    public Construct construct;
    public float currentTimeScale;
    public bool isConstructMode;

    

    public void Init()
    {
        GameManager.Subscribe(EVENT_TYPE.CONSTRUCT, SetConstructMode);
        currentTimeScale = 1f;
    }

    private void SetConstructMode()
    {
        Time.timeScale = (currentTimeScale == 1f) ? 0f : 1f;
        currentTimeScale = Time.timeScale;
        isConstructMode = (currentTimeScale == 1f) ? false : true;
    }



}
