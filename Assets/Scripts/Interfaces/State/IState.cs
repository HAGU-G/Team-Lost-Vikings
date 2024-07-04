using UnityEngine;

public interface IState
{ 
    /// <summary>
    /// 상태 전환 가능 여부
    /// </summary>
    public bool CanExit { get; protected set; }

    public void EnterState();
    public void ExitState();

    /// <summary>
    /// 
    /// </summary>
    public void ResetState();

}