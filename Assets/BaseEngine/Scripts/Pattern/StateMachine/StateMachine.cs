using System;
using System.Collections.Generic;

public class StateMachine
{
    private IState currentState;
    protected Dictionary<Type, IState> states = new();

    protected virtual void InitStates()
    {
        // Initialize all states here
    }

    public void ChangeState<T>() where T : IState
    {
        if (IsState<T>()) return;
        if (states.TryGetValue(typeof(T), out var newState))
        {
            currentState?.ExitState();
            currentState = newState;
            currentState.EnterState();
        }
    }
    private bool IsState<T>() where T : IState
    {
        return currentState is T;
    }

    public void Update()
    {
        currentState?.UpdateState();
    }
}