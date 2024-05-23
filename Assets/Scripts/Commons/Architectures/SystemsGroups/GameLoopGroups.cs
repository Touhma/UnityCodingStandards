using Unity.Entities;

namespace Commons.Architectures {
    // Could be more fleshed out for more system groups depending on the projects needs.
    [UpdateBefore(typeof(ExecutionSystemGroup))]
    public partial class InitializationSystemGroup : ComponentSystemGroup { }

    public partial class ExecutionSystemGroup : ComponentSystemGroup { }
}