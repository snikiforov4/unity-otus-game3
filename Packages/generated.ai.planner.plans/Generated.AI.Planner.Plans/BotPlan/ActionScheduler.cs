using System;
using Unity.AI.Planner;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using Unity.AI.Planner.Jobs;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Generated.AI.Planner.StateRepresentation;
using Generated.AI.Planner.StateRepresentation.BotPlan;

namespace Generated.AI.Planner.Plans.BotPlan
{
    public struct ActionScheduler :
        ITraitBasedActionScheduler<TraitBasedObject, StateEntityKey, StateData, StateDataContext, StateManager, ActionKey>
    {
        public static readonly Guid AttackEnemyGuid = Guid.NewGuid();
        public static readonly Guid FindAmmoGuid = Guid.NewGuid();
        public static readonly Guid FindHealthGuid = Guid.NewGuid();
        public static readonly Guid NavigateToEnemyGuid = Guid.NewGuid();

        // Input
        public NativeList<StateEntityKey> UnexpandedStates { get; set; }
        public StateManager StateManager { get; set; }

        // Output
        NativeQueue<StateTransitionInfoPair<StateEntityKey, ActionKey, StateTransitionInfo>> IActionScheduler<StateEntityKey, StateData, StateDataContext, StateManager, ActionKey>.CreatedStateInfo
        {
            set => m_CreatedStateInfo = value;
        }

        NativeQueue<StateTransitionInfoPair<StateEntityKey, ActionKey, StateTransitionInfo>> m_CreatedStateInfo;

        struct PlaybackECB : IJob
        {
            public ExclusiveEntityTransaction ExclusiveEntityTransaction;

            [ReadOnly]
            public NativeList<StateEntityKey> UnexpandedStates;
            public NativeQueue<StateTransitionInfoPair<StateEntityKey, ActionKey, StateTransitionInfo>> CreatedStateInfo;
            public EntityCommandBuffer AttackEnemyECB;
            public EntityCommandBuffer FindAmmoECB;
            public EntityCommandBuffer FindHealthECB;
            public EntityCommandBuffer NavigateToEnemyECB;

            public void Execute()
            {
                // Playback entity changes and output state transition info
                var entityManager = ExclusiveEntityTransaction;

                AttackEnemyECB.Playback(entityManager);
                for (int i = 0; i < UnexpandedStates.Length; i++)
                {
                    var stateEntity = UnexpandedStates[i].Entity;
                    var AttackEnemyRefs = entityManager.GetBuffer<AttackEnemyFixupReference>(stateEntity);
                    for (int j = 0; j < AttackEnemyRefs.Length; j++)
                        CreatedStateInfo.Enqueue(AttackEnemyRefs[j].TransitionInfo);
                    entityManager.RemoveComponent(stateEntity, typeof(AttackEnemyFixupReference));
                }

                FindAmmoECB.Playback(entityManager);
                for (int i = 0; i < UnexpandedStates.Length; i++)
                {
                    var stateEntity = UnexpandedStates[i].Entity;
                    var FindAmmoRefs = entityManager.GetBuffer<FindAmmoFixupReference>(stateEntity);
                    for (int j = 0; j < FindAmmoRefs.Length; j++)
                        CreatedStateInfo.Enqueue(FindAmmoRefs[j].TransitionInfo);
                    entityManager.RemoveComponent(stateEntity, typeof(FindAmmoFixupReference));
                }

                FindHealthECB.Playback(entityManager);
                for (int i = 0; i < UnexpandedStates.Length; i++)
                {
                    var stateEntity = UnexpandedStates[i].Entity;
                    var FindHealthRefs = entityManager.GetBuffer<FindHealthFixupReference>(stateEntity);
                    for (int j = 0; j < FindHealthRefs.Length; j++)
                        CreatedStateInfo.Enqueue(FindHealthRefs[j].TransitionInfo);
                    entityManager.RemoveComponent(stateEntity, typeof(FindHealthFixupReference));
                }

                NavigateToEnemyECB.Playback(entityManager);
                for (int i = 0; i < UnexpandedStates.Length; i++)
                {
                    var stateEntity = UnexpandedStates[i].Entity;
                    var NavigateToEnemyRefs = entityManager.GetBuffer<NavigateToEnemyFixupReference>(stateEntity);
                    for (int j = 0; j < NavigateToEnemyRefs.Length; j++)
                        CreatedStateInfo.Enqueue(NavigateToEnemyRefs[j].TransitionInfo);
                    entityManager.RemoveComponent(stateEntity, typeof(NavigateToEnemyFixupReference));
                }
            }
        }

        public JobHandle Schedule(JobHandle inputDeps)
        {
            var entityManager = StateManager.EntityManager;
            var AttackEnemyDataContext = StateManager.GetStateDataContext();
            var AttackEnemyECB = StateManager.GetEntityCommandBuffer();
            AttackEnemyDataContext.EntityCommandBuffer = AttackEnemyECB.ToConcurrent();
            var FindAmmoDataContext = StateManager.GetStateDataContext();
            var FindAmmoECB = StateManager.GetEntityCommandBuffer();
            FindAmmoDataContext.EntityCommandBuffer = FindAmmoECB.ToConcurrent();
            var FindHealthDataContext = StateManager.GetStateDataContext();
            var FindHealthECB = StateManager.GetEntityCommandBuffer();
            FindHealthDataContext.EntityCommandBuffer = FindHealthECB.ToConcurrent();
            var NavigateToEnemyDataContext = StateManager.GetStateDataContext();
            var NavigateToEnemyECB = StateManager.GetEntityCommandBuffer();
            NavigateToEnemyDataContext.EntityCommandBuffer = NavigateToEnemyECB.ToConcurrent();

            var allActionJobs = new NativeArray<JobHandle>(5, Allocator.TempJob)
            {
                [0] = new AttackEnemy(AttackEnemyGuid, UnexpandedStates, AttackEnemyDataContext).Schedule(UnexpandedStates, 0, inputDeps),
                [1] = new FindAmmo(FindAmmoGuid, UnexpandedStates, FindAmmoDataContext).Schedule(UnexpandedStates, 0, inputDeps),
                [2] = new FindHealth(FindHealthGuid, UnexpandedStates, FindHealthDataContext).Schedule(UnexpandedStates, 0, inputDeps),
                [3] = new NavigateToEnemy(NavigateToEnemyGuid, UnexpandedStates, NavigateToEnemyDataContext).Schedule(UnexpandedStates, 0, inputDeps),
                [4] = entityManager.ExclusiveEntityTransactionDependency
            };

            var allActionJobsHandle = JobHandle.CombineDependencies(allActionJobs);
            allActionJobs.Dispose();

            // Playback entity changes and output state transition info
            var playbackJob = new PlaybackECB()
            {
                ExclusiveEntityTransaction = StateManager.ExclusiveEntityTransaction,
                UnexpandedStates = UnexpandedStates,
                CreatedStateInfo = m_CreatedStateInfo,
                AttackEnemyECB = AttackEnemyECB,
                FindAmmoECB = FindAmmoECB,
                FindHealthECB = FindHealthECB,
                NavigateToEnemyECB = NavigateToEnemyECB,
            };

            var playbackJobHandle = playbackJob.Schedule(allActionJobsHandle);
            entityManager.ExclusiveEntityTransactionDependency = playbackJobHandle;

            return playbackJobHandle;
        }
    }
}
