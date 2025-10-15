using System;
using System.Collections;
using ProjectDawn.Navigation;
using ProjectDawn.Navigation.Hybrid;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    [SerializeField] private AgentType agentType;
    public AgentType AgentType => agentType;
    private AgentAuthoring agent;
    private Coroutine _curMoveCoroutine;

    private void Awake()
    {
        agent = GetComponent<AgentAuthoring>();
    }

    public void MoveTo(Movable payload)
    {
        StopCoroutine(_curMoveCoroutine);
        StartCoroutine(MoveToPosition(payload));
    }


    private IEnumerator MoveToPosition(Movable payload)
    {
        var destination = payload.destination;
        var finishedAction = payload.finishedAction;
        agent.SetDestination(destination);
        while (!IsReachedDestination(destination))
        {
            yield return null;
        }
        EventBus.Publish(GameEvents.MovementEvents.OnMoveFinished, finishedAction);
    }




    public bool IsReachedDestination(Vector3 destination)
    {
        float SMALLEST_SQRT_DISTANCE = agent.DefaultLocomotion.StoppingDistance;
        return Vector3.SqrMagnitude(agent.transform.position - destination) < SMALLEST_SQRT_DISTANCE;
    }
    public float GetRemainingDistance()
    {
        return agent.Body.RemainingDistance;
    }
    public void ResetAgent()
    {
        //StopMoving();

    }

}
