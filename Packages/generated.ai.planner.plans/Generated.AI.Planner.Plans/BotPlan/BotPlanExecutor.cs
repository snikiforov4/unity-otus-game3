using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Planner;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using Unity.AI.Planner.Controller;
using UnityEngine;
using UnityEngine.AI.Planner.DomainLanguage.TraitBased;
using Generated.AI.Planner.StateRepresentation;
using Generated.AI.Planner.StateRepresentation.BotPlan;

namespace Generated.AI.Planner.Plans.BotPlan
{
    public struct DefaultHeuristic : IHeuristic<StateData>
    {
        public BoundedValue Evaluate(StateData state)
        {
            return new BoundedValue(-100, 0, 100);
        }
    }

    public struct TerminationEvaluator : ITerminationEvaluator<StateData>
    {
        public bool IsTerminal(StateData state, out float terminalReward)
        {
            terminalReward = 0f;
            var terminal = false;
            
            var TerminationInstance = new Termination();
            if (TerminationInstance.IsTerminal(state))
            {
                terminal = true;
                terminalReward += TerminationInstance.TerminalReward(state);
            }
            return terminal;
        }
    }

    class BotPlanExecutor : BaseTraitBasedPlanExecutor<TraitBasedObject, StateEntityKey, StateData, StateDataContext, ActionScheduler, DefaultHeuristic, TerminationEvaluator, StateManager, ActionKey, DestroyStatesJobScheduler>
    {
        static Dictionary<Guid, string> s_ActionGuidToNameLookup = new Dictionary<Guid,string>()
        {
            { ActionScheduler.AttackEnemyGuid, nameof(AttackEnemy) },
            { ActionScheduler.FindAmmoGuid, nameof(FindAmmo) },
            { ActionScheduler.FindHealthGuid, nameof(FindHealth) },
            { ActionScheduler.NavigateToEnemyGuid, nameof(NavigateToEnemy) },
        };

        public override string GetActionName(IActionKey actionKey)
        {
            s_ActionGuidToNameLookup.TryGetValue(((IActionKeyWithGuid)actionKey).ActionGuid, out var name);
            return name;
        }

        public override void Initialize(MonoBehaviour actor, PlanDefinition planDefinition, IActionExecutionInfo[] actionExecutionInfos)
        {
            base.Initialize(actor, planDefinition, actionExecutionInfos);
            m_StateManager.Destroying += () => PlannerScheduler.CurrentJobHandle.Complete();
        }

        protected override void Act(ActionKey actionKey)
        {
            var stateData = m_StateManager.GetStateData(CurrentPlanState, false);
            var actionName = string.Empty;

            switch (actionKey.ActionGuid)
            {
                case var actionGuid when actionGuid == ActionScheduler.AttackEnemyGuid:
                    actionName = nameof(AttackEnemy);
                    break;
                case var actionGuid when actionGuid == ActionScheduler.FindAmmoGuid:
                    actionName = nameof(FindAmmo);
                    break;
                case var actionGuid when actionGuid == ActionScheduler.FindHealthGuid:
                    actionName = nameof(FindHealth);
                    break;
                case var actionGuid when actionGuid == ActionScheduler.NavigateToEnemyGuid:
                    actionName = nameof(NavigateToEnemy);
                    break;
            }

            var executeInfos = GetExecutionInfo(actionName);
            if (executeInfos == null)
                return;

            var argumentMapping = executeInfos.GetArgumentValues();
            var arguments = new object[argumentMapping.Count()];
            var i = 0;
            foreach (var argument in argumentMapping)
            {
                var split = argument.Split('.');

                int parameterIndex = -1;
                var traitBasedObjectName = split[0];

                if (string.IsNullOrEmpty(traitBasedObjectName))
                    throw new ArgumentException($"An argument to the '{actionName}' callback on '{m_Actor?.name}' DecisionController is invalid");

                switch (actionName)
                {
                    case nameof(AttackEnemy):
                        parameterIndex = AttackEnemy.GetIndexForParameterName(traitBasedObjectName);
                        break;
                    case nameof(FindAmmo):
                        parameterIndex = FindAmmo.GetIndexForParameterName(traitBasedObjectName);
                        break;
                    case nameof(FindHealth):
                        parameterIndex = FindHealth.GetIndexForParameterName(traitBasedObjectName);
                        break;
                    case nameof(NavigateToEnemy):
                        parameterIndex = NavigateToEnemy.GetIndexForParameterName(traitBasedObjectName);
                        break;
                }

                if (parameterIndex == -1)
                    throw new ArgumentException($"Argument '{traitBasedObjectName}' to the '{actionName}' callback on '{m_Actor?.name}' DecisionController is invalid");

                var traitBasedObjectIndex = actionKey[parameterIndex];
                if (split.Length > 1) // argument is a trait
                {
                    switch (split[1])
                    {
                        case nameof(Agent):
                            var traitAgent = stateData.GetTraitOnObjectAtIndex<Agent>(traitBasedObjectIndex);
                            arguments[i] = split.Length == 3 ? traitAgent.GetField(split[2]) : traitAgent;
                            break;
                        case nameof(Location):
                            var traitLocation = stateData.GetTraitOnObjectAtIndex<Location>(traitBasedObjectIndex);
                            arguments[i] = split.Length == 3 ? traitLocation.GetField(split[2]) : traitLocation;
                            break;
                        case nameof(Target):
                            var traitTarget = stateData.GetTraitOnObjectAtIndex<Target>(traitBasedObjectIndex);
                            arguments[i] = split.Length == 3 ? traitTarget.GetField(split[2]) : traitTarget;
                            break;
                        case nameof(Collectible):
                            var traitCollectible = stateData.GetTraitOnObjectAtIndex<Collectible>(traitBasedObjectIndex);
                            arguments[i] = split.Length == 3 ? traitCollectible.GetField(split[2]) : traitCollectible;
                            break;
                    }
                }
                else // argument is an object
                {
                    var planStateId = stateData.GetTraitBasedObjectId(traitBasedObjectIndex);
                    ITraitBasedObjectData dataSource;
                    if (m_PlanStateToGameStateIdLookup.TryGetValue(planStateId.Id, out var gameStateId))
                        dataSource = m_StateConverter.GetDataSource(new TraitBasedObjectId { Id = gameStateId });
                    else
                        dataSource = m_StateConverter.GetDataSource(planStateId);

                    Type expectedType = executeInfos.GetParameterType(i);
                    if (typeof(ITraitBasedObjectData).IsAssignableFrom(expectedType))
                    {
                        arguments[i] = dataSource;
                    }
                    else
                    {
                        arguments[i] = null;
                        var obj = dataSource.ParentObject;
                        if (obj != null && obj is GameObject gameObject)
                        {
                            if (expectedType == typeof(GameObject))
                                arguments[i] = gameObject;

                            if (typeof(Component).IsAssignableFrom(expectedType))
                                arguments[i] = gameObject == null ? null : gameObject.GetComponent(expectedType);
                        }
                    }
                }

                i++;
            }

            CurrentActionKey = actionKey;
            StartAction(executeInfos, arguments);
        }

        public override IActionParameterInfo[] GetActionParametersInfo(IStateKey stateKey, IActionKey actionKey)
        {
            string[] parameterNames = {};
            var stateData = m_StateManager.GetStateData((StateEntityKey)stateKey, false);

            switch (((IActionKeyWithGuid)actionKey).ActionGuid)
            {
                 case var actionGuid when actionGuid == ActionScheduler.AttackEnemyGuid:
                    parameterNames = AttackEnemy.parameterNames;
                        break;
                 case var actionGuid when actionGuid == ActionScheduler.FindAmmoGuid:
                    parameterNames = FindAmmo.parameterNames;
                        break;
                 case var actionGuid when actionGuid == ActionScheduler.FindHealthGuid:
                    parameterNames = FindHealth.parameterNames;
                        break;
                 case var actionGuid when actionGuid == ActionScheduler.NavigateToEnemyGuid:
                    parameterNames = NavigateToEnemy.parameterNames;
                        break;
            }

            var parameterInfo = new IActionParameterInfo[parameterNames.Length];
            for (var i = 0; i < parameterNames.Length; i++)
            {
                var traitBasedObjectId = stateData.GetTraitBasedObjectId(((ActionKey)actionKey)[i]);

#if DEBUG
                parameterInfo[i] = new ActionParameterInfo { ParameterName = parameterNames[i], TraitObjectName = traitBasedObjectId.Name.ToString(), TraitObjectId = traitBasedObjectId.Id };
#else
                parameterInfo[i] = new ActionParameterInfo { ParameterName = parameterNames[i], TraitObjectName = traitBasedObjectId.ToString(), TraitObjectId = traitBasedObjectId.Id };
#endif
            }

            return parameterInfo;
        }
    }
}
