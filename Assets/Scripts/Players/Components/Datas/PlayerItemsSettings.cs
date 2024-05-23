using Unity.Entities;

namespace Players.Components {
    public struct PlayerItemsSettings : IComponentData {
        public int ItemID;
        public int MaxStack;
    }
}