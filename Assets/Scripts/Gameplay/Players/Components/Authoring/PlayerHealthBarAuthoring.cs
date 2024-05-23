using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Players.Components.Authoring {
    public class PlayerHealthBarAuthoring : MonoBehaviour {
        public PlayerHealthBarManagedData Data;
        public class PlayerHealthBarAuthoringBaker : Baker<PlayerHealthBarAuthoring> {
            public override void Bake(PlayerHealthBarAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponentObject(entity, authoring.Data);
                AddComponent<PlayerHealthBarSingletonTag>(entity);
            }
        }
    }

    [Serializable]
    public class PlayerHealthBarManagedData : IComponentData {
        public GameObject HeathBarGameObject;
        public Image HeathBarImage;
    }

    public struct PlayerHealthBarSingletonTag : IComponentData { }
}