using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Coroutine followPathCoroutine;

    // Config for the agent
    private const float SMALL_DISTANCE = 0.1f;

    // call backs
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }


    // functions 

    public void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    public void Stop()
    {
        agent.isStopped = true;
    }

    public void SetAndFollowPath(Vector3[] path)
    {
        if (followPathCoroutine != null)
        {
            StopCoroutine(followPathCoroutine);
        }
        followPathCoroutine = StartCoroutine(followPath(path));
    }

    private IEnumerator followPath(Vector3[] path)
    {
        print("Start following path");
        foreach (Vector3 point in path)
        {
            agent.SetDestination(point);
            yield return new WaitUntil(IsReachedDestination);
        }
        print("Reached destination");
    }

    private bool IsReachedDestination()
    {
        return agent.remainingDistance < SMALL_DISTANCE;
    }




}
