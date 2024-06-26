using Commons.Architectures;
using Players.Components;
using Unity.Burst;
using Unity.Entities;

namespace Players.Systems {
    // Example of unmanaged Burstable System
    [UpdateInGroup(typeof(ExecutionSystemGroup))]
    public partial struct PlayerDoingOtherStuffSystem : ISystem {
        private static EntityQuery Query => EntityFactories.PlayerEntityFactory.Query;

        public void OnCreate(ref SystemState state) {
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            
            new PlayerStuffJob().Schedule(Query);
        }

        [BurstCompile]
        public partial struct PlayerStuffJob : IJobEntity {
            [BurstCompile]
            public static void Execute(ref PositionComponent component) {
                // DoSomething
                component.Value.x++;
            }
        }
    }
}