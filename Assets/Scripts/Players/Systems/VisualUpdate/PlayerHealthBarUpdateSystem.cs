using Players.Components.Authoring;
using Unity.Entities;
using UnityEngine;

namespace Players.Systems {
    // Example of managed System
    [RequireMatchingQueriesForUpdate]
    public partial class PlayerHealthBarUpdateSystem : SystemBase {
        protected override void OnUpdate() {
            // Logic to update the managed UI & getting the gameobject reference
            
            // Method 1
            Entity healthBar = SystemAPI.GetSingletonEntity<PlayerHealthBarSingletonTag>();
            GameObject heathBarGameObject = SystemAPI.ManagedAPI.GetComponent<PlayerHealthBarManagedData>(healthBar).HeathBarGameObject;
            
            //Method 2 if PlayerHealthBarManagedData is a singleton
            GameObject heathBarGameObjectSecond  = SystemAPI.ManagedAPI.GetSingleton<PlayerHealthBarManagedData>().HeathBarGameObject; 
        }
    }
}