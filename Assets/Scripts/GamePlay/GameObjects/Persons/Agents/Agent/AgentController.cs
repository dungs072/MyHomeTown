using System;
using System.Collections;
using ProjectDawn.Navigation;
using ProjectDawn.Navigation.Hybrid;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    [SerializeField] private AgentType agentType;
    [SerializeField] private Transform target;
    public AgentType AgentType => agentType;
    private AgentAuthoring agent;

    public void SetTarget(Transform target)
    {
        this.target = target;
        SetDestination(target.position);
    }

    private void Awake()
    {
        agent = GetComponent<AgentAuthoring>();
    }



    void Start()
    {
        if (target == null) return;
        SetDestination(target.position);
    }

    public void SetDestination(Vector3 destination)
    {
        agent.SetDestinationDeferred(destination);
    }
    public void StopMoving()
    {
        agent.Stop();
    }
    /// <summary>
    /// Move to specific position with coroutine. When the coroutine finished, 
    /// the agent is reached the destination
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>

    public IEnumerator MoveToPosition(Vector3 destination, Func<bool> shouldStopMoving = null, Action moveFinished = null)
    {
        SetDestination(destination);
        while (!IsReachedDestination(destination))
        {
            if (shouldStopMoving != null && shouldStopMoving())
            {
                agent.Stop();
                yield break;
            }
            yield return null;
        }
        moveFinished?.Invoke();
    }




    public bool IsReachedDestination(Vector3 destination)
    {
        float SMALLEST_SQRT_DISTANCE = agent.DefaultLocomotion.StoppingDistance;
        return Vector3.SqrMagnitude(agent.transform.position - destination) < SMALLEST_SQRT_DISTANCE;
    }
    public void ResetAgent()
    {
        StopMoving();

    }

}
