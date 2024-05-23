﻿using Commons.Architectures;
using Players.Settings.ScriptableObjects;
using Unity.Entities;

namespace Players.Systems {
    // Example of managed System
    public partial class PlayerManagedSettingsSystem : SystemBase {
        protected override void OnUpdate() {
            PlayerStatsSettingsScriptable playerStatsSettingsScriptable = ManagedSettings.PlayerSettings.GetSettings<PlayerStatsSettingsScriptable>(); // Getting scriptable reference -> Not Burst Compatible
            int itemID = UnmanagedSettings.PlayerItemSettingsDB.DB[0].ItemID; // Getting item ID from the unmanaged Item DB -> Burst Compatible
        }
    }
}