using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentController : MonoBehaviour
{
    [SerializeField] private bool canGoBack = false;
    [SerializeField] private bool canUsePool = false;
    private NavMeshAgent agent;
    private Coroutine followPathCoroutine;

    // Config for the agent
    private const float SMALL_DISTANCE = 1f;

    public void SetCanGoBack(bool canGoBack)
    {
        this.canGoBack = canGoBack;
    }
    public void SetCanUsePool(bool canUsePool)
    {
        this.canUsePool = canUsePool;
    }

    // call backs
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }


    // functions 

    public void SetDestination(Vector3 destination)
    {
        agent.isStopped = false;
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
        foreach (Vector3 point in path)
        {
            agent.SetDestination(point);
            yield return new WaitUntil(() => IsReachedDestination(point));
        }
        if (!canGoBack) yield break;
        for (int i = path.Length - 1; i >= 0; i--)
        {
            agent.SetDestination(path[i]);
            yield return new WaitUntil(() => IsReachedDestination(path[i]));
        }

        this.gameObject.SetActive(!canUsePool);
    }

    public bool IsReachedDestination(Vector3 currentDestination)
    {
        return Vector3.SqrMagnitude(transform.position - currentDestination) < SMALL_DISTANCE * SMALL_DISTANCE;
    }




}
