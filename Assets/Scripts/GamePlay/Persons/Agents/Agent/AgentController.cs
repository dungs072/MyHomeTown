using System;
using System.Collections;
using ProjectDawn.Navigation.Hybrid;
using UnityEngine;


public class AgentController : MonoBehaviour
{
    [SerializeField] private Transform target;
    private AgentAuthoring agent;

    public void SetTarget(Transform target)
    {
        this.target = target;
        agent.SetDestination(target.position);
    }

    private void Awake()
    {
        agent = GetComponent<AgentAuthoring>();
    }


    void Start()
    {
        if (target == null) return;
        agent.SetDestination(target.position);
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
        agent.SetDestination(destination);
        while (!IsReachedDestination(destination))
        {
            if (shouldStopMoving != null && shouldStopMoving())
            {
                yield break;
            }
            yield return null;
        }
        moveFinished?.Invoke();
    }




    public bool IsReachedDestination(Vector3 destination)
    {
        float SMALLEST_SQRT_DISTANCE = 0.2f;
        return Vector3.SqrMagnitude(agent.transform.position - destination) < SMALLEST_SQRT_DISTANCE;
    }

}
