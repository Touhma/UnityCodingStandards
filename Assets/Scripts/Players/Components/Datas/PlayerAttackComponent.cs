using System;
using Unity.Entities;

namespace Players.Components {
    
    // Serializable cause used in the Scriptable Object 
    
    [Serializable] 
    public struct PlayerAttackComponent : IComponentData {
        public float Value;
    }
    
    [Serializable]
    public struct PlayerAttackCountComponent : IComponentData {
        public int Value;
    }
    
    [Serializable]
    public struct PlayerAttackSpeedComponent : IComponentData {
        public int Value;
    }
}