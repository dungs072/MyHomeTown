using System;
using System.Collections.Generic;
using System.Linq;

public enum AgentType
{
    CUSTOMER = 1,
    SERVER = 2,
    RECEPTIONIST = 3,
}
public class AgentTypeList
{
    private static List<AgentType> agentTypes = Enum.GetValues(typeof(AgentType))
                                           .Cast<AgentType>()
                                           .ToList();
    public static List<AgentType> AgentTypes
    {
        get { return agentTypes; }
    }

}
