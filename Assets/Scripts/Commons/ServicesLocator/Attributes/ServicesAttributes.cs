using System;

namespace Commons.ServicesLocator {
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class InitializeBefore : Attribute {
        public Type TargetServiceType { get; }

        public InitializeBefore(Type targetServiceType) => TargetServiceType = targetServiceType;
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class InitializeInGroup : Attribute {
        public Type GroupType { get; }

        public InitializeInGroup(Type groupType) => GroupType = groupType;
    }
}