using Unity.Entities;
using Unity.Mathematics;

namespace Players.Components {
    public struct PositionComponent : IComponentData {
        public float3 Value;
    }

    public struct HealthComponent : IComponentData {
        public int Value;
    }

    public struct VelocityComponent : IComponentData {
        public float Value;
    }
}