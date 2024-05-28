using System;
using System.Collections.Generic;
using Commons.GameLoop;
using Commons.Services;
using JetBrains.Annotations;
using UnityEngine;

namespace Players.Settings.Databases {
    [UsedImplicitly]
    [InitializeInGroup(typeof(GameLoopDatabasesInitialisationGroup))]
    public class PlayerSettingsManagedDB : IServiceBase {
        public readonly Dictionary<Type, ScriptableObject> Settings = new();

        public string SettingPath = "Settings/Players";

        public void Initialize() {
            Debug.Log("Initialize -> " + SettingPath + " -> " + Resources.LoadAll<ScriptableObject>(SettingPath).Length);

            foreach (ScriptableObject scriptableObject in Resources.LoadAll<ScriptableObject>(SettingPath)) {
                Settings.Add(scriptableObject.GetType(), scriptableObject);
            }
        }

        public void Destroy() {
            Settings.Clear();
        }

        public T GetSettings<T>() where T : ScriptableObject {
            return Settings.TryGetValue(typeof(T), out ScriptableObject setting) ? setting as T : null;
        }
    }
}