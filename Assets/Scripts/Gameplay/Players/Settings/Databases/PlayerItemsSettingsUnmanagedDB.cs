using System;
using Commons.Architectures;
using Players.Components;
using Unity.Collections;


//*
namespace Players.Settings.Databases {
    // this is an example of an unmanaged DB, could be usefull in certain situation when these settings need to be accessed from burst jobs ( Like crafting elements, recipes etc ... ) 
    [UnmanagedSetting] // This will create a UnmanagedSetting shared static & it's shorcut so you can call : UnmanagedSetting.PlayerItemsSettingsUnmanagedDB directly
    public struct PlayerItemsSettingsUnmanagedDB : IDisposable {
        
        public NativeHashMap<int, PlayerItemsSettings> DB;

        public PlayerItemsSettingsUnmanagedDB(int bufferLength) => DB = new NativeHashMap<int, PlayerItemsSettings>(bufferLength, Allocator.Persistent);

        public PlayerItemsSettings GetSetting(int id) => DB[id];

        public void AddSetting(PlayerItemsSettings setting) => DB.Add(setting.ItemID, setting);

        public void Dispose() => DB.Dispose();
    }
    
    [UnmanagedSetting] // This will create a UnmanagedSetting shared static & it's shorcut so you can call : UnmanagedSetting.PlayerItemsSettingsUnmanagedDB directly
    public struct PlayerItemsSettingsUnmanagedDB2 : IDisposable {
        
        public NativeHashMap<int, PlayerItemsSettings> DB;

        public PlayerItemsSettingsUnmanagedDB2(int bufferLength) => DB = new NativeHashMap<int, PlayerItemsSettings>(bufferLength, Allocator.Persistent);

        public PlayerItemsSettings GetSetting(int id) => DB[id];

        public void AddSetting(PlayerItemsSettings setting) => DB.Add(setting.ItemID, setting);

        public void Dispose() => DB.Dispose();
    }
}
//*/