using Players.Factories;
using Unity.Entities;

namespace Commons.Architectures {
    [UpdateInGroup(typeof(Unity.Entities.InitializationSystemGroup))]
    public partial struct EntitiesFactoryInitializationSystem : ISystem {
        public void OnCreate(ref SystemState state) { // Need to setup the factory once 
            EntityFactories.PlayerEntityFactory = new PlayerEntityFactory(ref state); // this could be codegen or else need to be manually added
            state.Enabled = false;
        }
    }
}