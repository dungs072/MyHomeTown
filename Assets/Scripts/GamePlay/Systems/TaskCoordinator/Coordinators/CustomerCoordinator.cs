using System;
using System.Collections.Generic;

public class CustomerCoordinator
{
    private List<TaskHandler> customers = new();

    public void AddCustomer(TaskHandler customer)
    {
        customers.Add(customer);
    }

    public void AddSameTasksToAllCustomers(List<TaskName> taskNames)
    {
        foreach (var customer in customers)
        {
            AddTaskToCustomer(customer, taskNames);
        }
    }
    private void AddTaskToCustomer(TaskHandler customer, List<TaskName> taskNames)
    {
        foreach (var taskName in taskNames)
        {
            customer.AddTask(taskName);

        }
    }
    public void AddRandomTasksToAllCustomers(List<TaskName> taskNames)
    {
        foreach (var customer in customers)
        {
            var randomIndex = UnityEngine.Random.Range(0, taskNames.Count);
            var taskName = taskNames[randomIndex];
            AddTaskToCustomer(customer, taskName);
        }
    }
    private void AddTaskToCustomer(TaskHandler customer, TaskName taskName)
    {
        customer.AddTask(taskName);
    }

}