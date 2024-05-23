using Unity.Entities;

namespace Players.Components {
    public struct PlayerIsAttacking : IComponentData, IEnableableComponent { }
    public struct PlayerIsDefending : IComponentData, IEnableableComponent { }
}