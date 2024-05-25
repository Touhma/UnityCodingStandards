using System;

namespace Commons.Architectures
{
    [AttributeUsage(AttributeTargets.Struct)]
    public class EntityFactory : Attribute
    {
    }
    
    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false)]
    public class GenWith : Attribute
    {
        public Type[] ComponentTypes { get; }

        public GenWith(params Type[] componentTypes)
        {
            ComponentTypes = componentTypes;
        }
    }
}