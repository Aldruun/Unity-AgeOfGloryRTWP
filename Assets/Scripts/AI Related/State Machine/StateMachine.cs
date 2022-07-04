using UnityEngine;

public class StateMachine<T>
{
    public State<T> currentState;

    public void Update(T owner)
    {
        if(currentState != null)
            currentState.Execute(owner);
        else
        {
            Debug.Log("<color=red>Current state null</color>");
        }
    }

    public void ChangeState(State<T> newState, T owner)
    {
        if(currentState != null && currentState != newState)
        {
            currentState.Exit(owner);
        }

        currentState = newState;
        currentState.Enter(owner);
    }
}
public abstract class State<T>
{
    public abstract void Enter(T owner);
    public abstract void Execute(T owner);
    public abstract void Exit(T owner);
}
