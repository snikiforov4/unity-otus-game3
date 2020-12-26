using Unity.AI.Planner;
using Unity.Collections;
using Unity.Entities;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using Generated.AI.Planner.StateRepresentation;
using Generated.AI.Planner.StateRepresentation.BotPlan;
using Generated.AI.Planner.StateRepresentation.Enums;

namespace Generated.AI.Planner.Plans.BotPlan
{
    public struct Termination
    {
        public bool IsTerminal(StateData stateData)
        {
            var AgentFilter = new NativeArray<ComponentType>(1, Allocator.Temp){[0] = ComponentType.ReadWrite<Agent>(),  };
            var AgentObjectIndices = new NativeList<int>(2, Allocator.Temp);
            stateData.GetTraitBasedObjectIndices(AgentObjectIndices, AgentFilter);
            var AgentBuffer = stateData.AgentBuffer;
            for (int i0 = 0; i0 < AgentObjectIndices.Length; i0++)
            {
                var AgentIndex = AgentObjectIndices[i0];
                var AgentObject = stateData.TraitBasedObjects[AgentIndex];
            
                
                if (!(AgentBuffer[AgentObject.AgentIndex].Health <= 0))
                    continue;
                AgentObjectIndices.Dispose();
                AgentFilter.Dispose();
                return true;
            }
            AgentObjectIndices.Dispose();
            AgentFilter.Dispose();

            return false;
        }

        public float TerminalReward(StateData stateData)
        {
            var reward = 0f;

            return reward;
        }
    }
}
