using Players.Factories;
using Unity.Entities;

namespace Commons.Architectures {
    
    // Need to be codegen 
    [UpdateInGroup(typeof(Unity.Entities.InitializationSystemGroup))]
    public partial struct EntitiesFactoryInitializationSystem : ISystem {
        public void OnCreate(ref SystemState state) { // Need to setup the factory once 
            EntityFactories.PlayerEntityFactorySetup(ref state); 
            state.Enabled = false;
        }
    }
}