using System;
using Players.Components;
using Players.Settings.Databases;
using Unity.Burst;
using Unity.Collections;

namespace Commons.Architectures {
    public static partial class UnmanagedSettingsStatics { // Necessary for the partial to take effect - Could be codegen
        public static readonly SharedStatic<PlayerItemsSettingsUnmanagedDB> PlayerItemSettingsDB = SharedStatic<PlayerItemsSettingsUnmanagedDB>.GetOrCreate<PlayerItemsSettingsUnmanagedDB, StaticFieldKey>();
    }
    public static partial class UnmanagedSettings { // Necessary for the partial to take effect - Could be codegen
        public static PlayerItemsSettingsUnmanagedDB PlayerItemSettingsDB => UnmanagedSettingsStatics.PlayerItemSettingsDB.Data;
    }
}

namespace Players.Settings.Databases {
    // this is an example of an unmanaged DB, could be usefull in certain situation when these settings need to be accessed from burst jobs ( Like crafting elements, recipes etc ... ) 
    public struct PlayerItemsSettingsUnmanagedDB : IDisposable {
        
        public NativeHashMap<int, PlayerItemsSettings> DB;

        public PlayerItemsSettingsUnmanagedDB(int bufferLength) => DB = new NativeHashMap<int, PlayerItemsSettings>(bufferLength, Allocator.Persistent);

        public PlayerItemsSettings GetSetting(int id) => DB[id];

        public void AddSetting(PlayerItemsSettings setting) => DB.Add(setting.ItemID, setting);

        public void Dispose() => DB.Dispose();
    }
}