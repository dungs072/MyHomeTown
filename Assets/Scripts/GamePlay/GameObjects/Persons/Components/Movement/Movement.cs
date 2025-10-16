using System;
using System.Collections;
using ProjectDawn.Navigation;
using ProjectDawn.Navigation.Hybrid;
using UnityEngine;

public class Movement : MonoBehaviour
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
        if (_curMoveCoroutine != null)
        {
            StopCoroutine(_curMoveCoroutine);
        }
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
        finishedAction?.Invoke();
    }
    private bool IsReachedDestination(Vector3 destination)
    {
        float SMALLEST_SQRT_DISTANCE = 1.0f;
        return Vector3.SqrMagnitude(agent.transform.position - destination) < SMALLEST_SQRT_DISTANCE;
    }

    public void Stop()
    {
        agent.Stop();
        StopAllCoroutines();
    }
}
