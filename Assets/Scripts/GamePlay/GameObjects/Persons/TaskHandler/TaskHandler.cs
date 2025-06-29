using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ManagerSingleton;
//! This will handle all the tasks that are assigned to a person    
//! Must use yield return StartCoroutine(YourFunction()) to make sure the coroutine is running in sequence
[RequireComponent(typeof(AgentController))]
public class TaskHandler : MonoBehaviour
{


}