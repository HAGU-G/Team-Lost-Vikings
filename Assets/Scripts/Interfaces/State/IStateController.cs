using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.UI;

public abstract class IStateController
{
    private List<IState> states = new();
    private int defaultState = 0;
    private IState currentState;

    protected void ResetController()
    {
        foreach (var state in states)
        {
            state.ResetState();
        }
        currentState = null;
        ChangeState(states[defaultState]);
    }

    protected bool ChangeState(IState nextState)
    {
        if (!states.Contains(nextState))
        {
            return false;
        }

        if (currentState == null)
        {
            ChangeState(nextState);
            return true;
        }

        if (currentState.CanExit)
        {
            currentState.ExitState();
            nextState.EnterState();
            currentState = nextState;
            return true;
        }

        return false;
    }
}