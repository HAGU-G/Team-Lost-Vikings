using UnityEngine.UI;

public enum DIRECTION_VERTICAL
{
    MIDDLE,
    LEFT,
    RIGHT,
    BOTH
}

public enum DIRECTION_HORIZENTAL
{
    MIDDLE,
    UP,
    DOWN,
    BOTH
}

public enum BUTTON_SHOW_TYPE
{
    NONE,
    ACTIVE,
    DISABLE
}


public class DialogData : ITableAvaialable<int>
{
    public int Id { get; set; }
    public int DialogGroup { get; set; }
    public string SpeakerName { get; set; }
    public string ImageFileName { get; set; }
    public DIRECTION_VERTICAL ImageMarker { get; set; }
    public string DialogText { get; set; }
    public BUTTON_SHOW_TYPE SkipBtnEnable { get; set; }
    public int TableID => Id;
}