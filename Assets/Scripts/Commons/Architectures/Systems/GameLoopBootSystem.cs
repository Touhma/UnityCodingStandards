using Commons.GameLoop;
using Commons.Services;
using Unity.Entities;
using UnityEngine;

namespace Commons.Architectures {
    [UpdateInGroup(typeof(GameLoopServicesInitialisationGroup))]
    public partial struct GameLoopBootSystem : ISystem // First system to run in the game. Initialize the services.
    {
        public void OnCreate(ref SystemState state) {
            Debug.Log("GameLoopBootSystem - OnCreate ");
            Application.targetFrameRate = -1; // Unlock framerate , because why not ?
            InitializeServicesBase();
        }

        private static void InitializeServicesBase() => ServiceLocator.InitializeServicesBase();

        public void OnUpdate(ref SystemState state) {
            ServiceLocator.PostInitializeServicesBase();
            state.Enabled = false;
        }

        public void OnDestroy(ref SystemState state) => ServiceLocator.Current.DestroyServices();
    }
}