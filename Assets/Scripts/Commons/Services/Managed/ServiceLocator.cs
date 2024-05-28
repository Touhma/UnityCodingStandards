using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Commons.Services
{
    public class ServiceLocator
    {
        private readonly Dictionary<Type, IServiceBase> _services = new();

        private ServiceLocator()
        {
        }

        public static ServiceLocator Current { get; private set; }

        public static void Initialize()
        {
            Current = new ServiceLocator();
        }

        public void InitServices()
        {
            Debug.Log("InitServices");
            foreach (IServiceBase service in _services.Values)
            {
                service.Initialize();
            }
        }

        public void PostInitServices()
        {
            Debug.Log("PostInitServices");

            foreach (IServiceBase service in _services.Values)
            {
                service.PostInit();
            }
        }

        public void DestroyServices()
        {
            Debug.Log("DestroyServices");
            foreach (IServiceBase service in _services.Values)
            {
                service.Destroy();
            }
        }

        public T Get<T>() where T : IServiceBase
        {
            if (_services.ContainsKey(typeof(T)))
            {
                return (T)_services[typeof(T)];
            }

            Debug.LogError($"{typeof(T)} not registered with {GetType().Name}");
            throw new InvalidOperationException();
        }

        public void Register<T>() where T : IServiceBase, new()
        {
            T service = new T();
            // IBaseService service = ScriptableObject.CreateInstance<T> ();
            if (_services.ContainsKey(typeof(T)))
            {
                Debug.LogError($"Attempted to register service of type {typeof(T)} which is already registered with the {GetType().Name}.");
                return;
            }

            _services.Add(typeof(T), service);
            Debug.Log(typeof(T) + " successfully registered");
        }

        public void Register(Type type, IServiceBase service)
        {
            if (_services.ContainsKey(type))
            {
                Debug.LogError($"Attempted to register service of type {type.Name} - which is already registered with the {GetType().Name}.");
                return;
            }

            _services.Add(type, service);
            Debug.Log(type.Name + " successfully registered");
        }

        public void Unregister<T>() where T : IServiceBase
        {
            if (!_services.ContainsKey(typeof(T)))
            {
                Debug.LogError($"Attempted to unregister service of type {typeof(T)} which is not registered with the {GetType().Name}.");
                return;
            }

            _services.Remove(typeof(T));
        }
        
         public static void InitializeServicesBase()
        {
            Initialize();

            IEnumerable<Type> allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes());

            // Extract the service groups and sort them
            IEnumerable<Type> enumerable = allTypes as Type[] ?? allTypes.ToArray();
            List<Type> sortedGroups = SortTypes(enumerable.Where(t => typeof(IServiceGroup).IsAssignableFrom(t) && !t.IsInterface).ToList());

            if (sortedGroups == null)
            {
                Debug.LogError("Circular dependency detected in service group initialization order.");
                return;
            }

            // For each group, extract its services and sort them
            foreach (Type group in sortedGroups)
            {
                List<Type> sortedServices = SortTypes(enumerable.Where(t => IsServiceInGroup(t, group)).ToList());
                if (sortedServices == null)
                {
                    Debug.LogError("Circular dependency detected in service initialization order within group: " + group.Name);
                    return;
                }

                foreach (Type serviceType in sortedServices)
                {
                    if (Activator.CreateInstance(serviceType) is not IServiceBase instance) continue;

                    Debug.Log("Register " + instance.GetType());
                    Current.Register(instance.GetType(), instance);
                }
            }

            Current.InitServices();
        }
        
        public static void PostInitializeServicesBase()
        {
            Current.PostInitServices();
        }

        private static bool IsServiceInGroup(MemberInfo serviceType, Type groupType)
        {
            InitializeInGroup groupAttribute = serviceType.GetCustomAttribute<InitializeInGroup>();
            return groupAttribute != null && groupAttribute.GroupType == groupType;
        }

        private static List<Type> SortTypes(IEnumerable<Type> types)
        {
            List<Type> sortedTypes = new();
            HashSet<Type> visited = new();
            HashSet<Type> temporaryMarked = new();
            bool hasLoop = false;

            return types.Where(type => !visited.Contains(type)).Any(type => HasLoop(type, visited, temporaryMarked, sortedTypes)) ? null : sortedTypes;
        }

        private static bool HasLoop(Type type, ISet<Type> visited, ISet<Type> temporaryMarked, IList<Type> sortedTypes)
        {
            if (temporaryMarked.Contains(type))
            {
                return true; // A loop is detected
            }

            if (visited.Contains(type)) return false;

            temporaryMarked.Add(type);
            InitializeBefore initializeBeforeAttribute = type.GetCustomAttribute<InitializeBefore>();
            if (initializeBeforeAttribute != null)
            {
                if (HasLoop(initializeBeforeAttribute.TargetServiceType, visited, temporaryMarked, sortedTypes))
                {
                    return true;
                }
            }

            visited.Add(type);
            temporaryMarked.Remove(type);
            
            sortedTypes.Insert(0, type);

            return false;
        }
    }
}