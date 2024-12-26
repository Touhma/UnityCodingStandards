using System;
using System.Threading;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Commons.Services {
    public class PlayerService : IComponentData {
        public int Data;
    }

    public static class _ {
        private static EntityManager EM => World.DefaultGameObjectInjectionWorld.EntityManager;
        private static EntityQuery LocatorEntityQuery;
        private static EntityQuery PlayerServiceQuery;
        public static PlayerService PlayerService => PlayerServiceQuery.GetSingleton<PlayerService>();

        static _() {
            
            LocatorEntityQuery = EM.CreateEntityQuery(ComponentType.ReadWrite<Main.Context>());
            LocatorEntityQuery = EM.CreateEntityQuery(ComponentType.ReadWrite<Main.Context>());
            PlayerServiceQuery = EM.CreateEntityQuery(ComponentType.ReadWrite<PlayerService>());
        }

        public static void Register<T>(T elem) where T : class, IComponentData, new() {
            Debug.Assert(elem != null);
            EM.SetComponentData(LocatorEntityQuery.GetSingletonEntity(), elem);
        }
    }

    public class Gameplay : MonoBehaviour {
        public void Update() {
            Debug.Log(_.PlayerService.Data);
        }
    }


    public sealed class Main : MonoBehaviour {
        private static readonly ComponentType[] LocatorArchetypeDesc = { ComponentType.ReadWrite<Context>() };

        public static void Register<T>(T elem, Entity handle, EntityManager manager = default) where T : class, IComponentData, new() {
            Debug.Assert(elem != null);
            if (manager == default) {
                manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            }

            manager.SetComponentData(handle, elem);
        }

        public static T Locate<T>(EntityManager manager = default) where T : class {
            if (manager == default) {
                manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            }

            var q = manager.CreateEntityQuery(ComponentType.ReadWrite<Context>());
            return q.GetSingleton<T>();
        }

        public class Context : IComponentData {
            public ILogger Logger; // For my friends that love The Liskov Substitution Principle
            public World ClientWorld;
        }

        [Preserve]
        public sealed class CustomClientServerBootstrap : ClientServerBootstrap {
            public override bool Initialize(string defaultWorldName) {
                CreateLocalWorld(defaultWorldName);
                return true;
            }
        }
    }
}