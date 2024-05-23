using System;
using System.Collections.Generic;
using Commons.Architectures;
using Commons.ServicesLocator;
using JetBrains.Annotations;
using Players.Settings.Databases;
using UnityEngine;


namespace Commons.Architectures { // Necessary for the partial to take effect - Could be codegen
    public static partial class ManagedSettings {
        public static PlayerSettingsManagedDB PlayerSettings =>  ServiceLocator.Current.Get<PlayerSettingsManagedDB>();
    }
}

namespace Players.Settings.Databases {
    [UsedImplicitly]
    [InitializeInGroup(typeof(SettingsServiceGroup))]
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