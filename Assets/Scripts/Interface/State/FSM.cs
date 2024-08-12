using System.Collections.Generic;
using UnityEngine;

public class FSM<T>
{
    private T owner;

    private List<State<T>> states = new();
    private int defaultState = 0;
    private State<T> currentState;

    public void Init(T owner, int defaultState, params State<T>[] states)
    {
        this.owner = owner;
        this.states.Clear();
        foreach (var state in states)
        {
            state.Init(owner, this);
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
    public bool ChangeState(State<T> state)
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
        //Debug.Log($"{Time.time}: {currentState.ToString()}");

        return true;
    }

    public void Update()
    {
        currentState.Update();
    }
}