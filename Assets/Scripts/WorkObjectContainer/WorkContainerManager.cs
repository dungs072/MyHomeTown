using System.Collections.Generic;
using UnityEngine;

public class WorkContainerManager : MonoBehaviour
{
    [SerializeField] private List<WorkContainer> workContainers;

    public List<WorkContainer> WorkContainers => workContainers;
}
