using System;
using System.Collections.Generic;

public class PersonStateMachine : StateMachine
{
    private Person person;
    public PersonStateMachine(Person person)
    {
        this.person = person;
        InitStates();
    }
    protected override void InitStates()
    {
        states[typeof(IdleState)] = new IdleState(person);
        states[typeof(MoveState)] = new MoveState(person);
        states[typeof(WorkState)] = new WorkState(person);
        states[typeof(WaitState)] = new WaitState(person);
        states[typeof(PatrollingState)] = new PatrollingState(person);
    }

}