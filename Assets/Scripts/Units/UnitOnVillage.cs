using UnityEditor.Build;

public class UnitOnVillage : Unit
{
    private FSM<UnitOnVillage> fsm;

    private void Awake()
    {
        Init();
        ResetUnit();
    }

    protected override void Init()
    {
        base.Init();

        //작성할 코드
        fsm = new();
        fsm.Init(this, 0,
            new StateExample());
    }

    protected override void ResetUnit()
    {
        base.ResetUnit();   

        //작성할 코드
    }

    private void Update()
    {
        fsm.Update();
    }
}