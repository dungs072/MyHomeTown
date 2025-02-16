using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PathFinder))]
public class AgentController : MonoBehaviour
{
    [SerializeField] private Transform target;
    private PathFinder pathFinder;

    private void Awake()
    {
        pathFinder = GetComponent<PathFinder>();
    }

    private void Start()
    {
        StartCoroutine(Test());
    }
    private IEnumerator Test()
    {
        yield return new WaitForSeconds(2f);
        var targetPosition = Vector3.zero;
        if (target == null)
        {
            targetPosition = new Vector3(7.3f, 0, 17.4f);

        }
        else
        {
            targetPosition = target.position;
        }
        pathFinder.MoveToDestination(targetPosition);
    }




}
