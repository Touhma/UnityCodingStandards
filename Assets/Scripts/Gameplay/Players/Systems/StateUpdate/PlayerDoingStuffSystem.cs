using Commons.Architectures;
using Players.Components;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using EntityFactories = Commons.Architectures.EntityFactories;

namespace Players.Systems {
    // Example of unmanaged Burstable System
    [UpdateInGroup(typeof(ExecutionSystemGroup))]
    public partial struct PlayerDoingStuffSystem : ISystem {
        private EntityQuery _query => EntityFactories.PlayerEntityFactory.Query;

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            new PlayerStuffJob().Schedule(_query); // or like this 
        }

        [BurstCompile]
        public partial struct PlayerStuffJob : IJobEntity {
            [BurstCompile]
            public static void Execute(ref PositionComponent component) {
                // DoSomething
                Debug.Log("Test");
            }
        }
    }
}