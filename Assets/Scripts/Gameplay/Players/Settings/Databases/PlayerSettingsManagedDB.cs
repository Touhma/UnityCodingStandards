using System;
using System.Collections.Generic;
using Commons.Architectures;
using Commons.Services;
using Commons.Services.ServiceGroups;
using JetBrains.Annotations;
using UnityEngine;

namespace Players.Settings.Databases {
    [UsedImplicitly]
    [ManagedSetting] // This will create a ManagedSettings shortcut so you can call : ManagedSettings.PlayerSettingsManagedDB directly
    [InitializeInGroup(typeof(SettingsServiceGroup))] // use the ServiceGroup attributes for initialization of services or else it's not gonna register
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