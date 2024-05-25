using Commons.Architectures;
using Unity.Burst;
using Unity.Entities;

namespace Players.Systems {
    [UpdateInGroup(typeof(ExecutionSystemGroup))]
    [UpdateBefore(typeof(PlayerDoingStuffSystem))]
    public partial struct PlayerEntityInstantiationSystem : ISystem {
        // Example of System without the caching of the factory
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityFactories.PlayerEntityFactory.CreateEntity(ref state); // Then you can use it like this 
        }
    }
}