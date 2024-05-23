using JetBrains.Annotations;

namespace Commons.Architectures {
    [UsedImplicitly]
    public static partial class UnmanagedSettingsStatics { // Shared static container for unmanaged settings
        public abstract class StaticFieldKey { }
    }


    [UsedImplicitly]
    public static partial class UnmanagedSettings { } // Partial to serve as entry point for any unmanaged settings
}