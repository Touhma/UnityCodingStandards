using System;

namespace Commons.Architectures {
    [AttributeUsage(AttributeTargets.Class)]
    public class ManagedSetting : Attribute {}
    
    [AttributeUsage(AttributeTargets.Struct)]
    public class UnmanagedSetting : Attribute {}
}