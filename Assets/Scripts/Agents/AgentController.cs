using ProjectDawn.Navigation;
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





}
