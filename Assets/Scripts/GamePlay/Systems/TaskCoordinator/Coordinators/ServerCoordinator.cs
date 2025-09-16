using System.Collections.Generic;
using UnityEngine;

public class ServerCoordinator
{
    private List<TaskHandler> servers = new();

    public void AddServer(TaskHandler server)
    {
        servers.Add(server);
    }

    public void AddTaskToCustomer(List<TaskName> taskNames)
    {
        var freeServers = servers.FindAll(server => server.IsFree());
        var suitableServers = freeServers.Count == 0 ? servers : freeServers;
        var randomServerIndex = Random.Range(0, suitableServers.Count);
        var selectedServer = suitableServers[randomServerIndex];
        foreach (var taskName in taskNames)
        {
            selectedServer.AddTask(taskName);
        }
    }

}
