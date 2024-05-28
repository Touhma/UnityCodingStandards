using Commons.Services;

namespace Commons.Architectures {
    [InitializeBefore(typeof(GameplayServiceGroup))]
    public class SettingsServiceGroup : IServiceGroup { } // Managed Settings Database setups

    public class GameplayServiceGroup : IServiceGroup { } // Used for UI / Managed interaction with a need for local state outside of Systems 
}