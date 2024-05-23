using System;
using Players.Components;
using Players.Factories;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Players.Factories {
    public struct PlayerEntityFactory : IDisposable {
        public EntityArchetype Archetype;
        public EntityQuery Query;

        public PlayerEntityFactory(ref SystemState state) {
            NativeList<ComponentType> componentTypes = new(Allocator.Temp) {
                ComponentType.ReadWrite<PositionComponent>(),
                ComponentType.ReadWrite<HealthComponent>(),
                ComponentType.ReadWrite<PlayerTag>(),
                ComponentType.ReadWrite<PlayerStateEnabled>()
            };

            Archetype = state.EntityManager.CreateArchetype(componentTypes.AsArray()); // Caching the Archetype
            Query = new EntityQueryBuilder(Allocator.Temp).WithAll(ref componentTypes).Build(ref state); // Caching the Query matching the archetype

            componentTypes.Dispose();
        }

        public Entity CreatePlayer(ref SystemState state) => state.EntityManager.CreateEntity(Archetype); // Use this method to instanciate something with the same archetype

        public void Dispose() {
            Query.Dispose();
        }
    }
}


// ReSharper disable once CheckNamespace
namespace Commons.Architectures { 
    // namespace necessary for the partial to take effect
    public static partial class EntityFactoriesStatics {
        public static readonly SharedStatic<PlayerEntityFactory> PlayerEntityFactory = SharedStatic<PlayerEntityFactory>.GetOrCreate<PlayerEntityFactory, StaticFieldKey>();
    }

    public static partial class EntityFactories {
        public static ref PlayerEntityFactory PlayerEntityFactory => ref EntityFactoriesStatics.PlayerEntityFactory.Data;
    }
}