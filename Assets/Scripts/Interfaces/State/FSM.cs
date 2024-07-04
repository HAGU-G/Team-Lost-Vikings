using System.Collections.Generic;
using UnityEngine;

public class FSM<T>
{
    private T controller;
    private List<IState<T>> states = new();
    private int defaultState = 0;
    private IState<T> currentState;

    public void Init(T controller, int defaultState, params IState<T>[] states)
    {
        this.controller = controller;
        this.states.Clear();
        foreach (var state in states)
        {
            state.Init(controller);
            this.states.Add(state);
        }
        this.defaultState = defaultState;
    }

    public void ResetFSM()
    {
        foreach (var state in states)
        {
            state.ResetState();
        }
        currentState = null;
        ChangeState(states[defaultState]);
    }


    public bool ChangeState(int stateIndex)
    {
        return ChangeState(states[stateIndex]);
    }

    /// <returns>true : 변경 성공, false : 변경 실패</returns>
    public bool ChangeState(IState<T> state)
    {
        if (!states.Contains(state))
        {
            return false;
        }

        if (currentState != null)
        {
            currentState.ExitState();
        }

        state.EnterState();
        currentState = state;

        return true;
    }
}