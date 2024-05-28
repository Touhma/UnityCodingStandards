using Unity.Entities;
using Unity.Transforms;

// using Unity.Physics.Systems;

namespace Commons.GameLoop
{
    //Base Game System Groups
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(GameLoopPreTransformSystemGroup))]
    public partial class GameLoopSystemGroup : ComponentSystemGroup { } // The gameloop execute BEFORE the transform Update

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(GameLoopSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial class GameLoopPreTransformSystemGroup : ComponentSystemGroup { } // The gameloop execute BEFORE the transform Update

    [UpdateInGroup(typeof(GameLoopSystemGroup))]
    [UpdateBefore(typeof(GameLoopServicesInitialisationGroup))]
    public partial class GameLoopSingletonsInitialisationGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(GameLoopSystemGroup))]
    [UpdateBefore(typeof(GameLoopDatabasesInitialisationGroup))]
    public partial class GameLoopServicesInitialisationGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(GameLoopSystemGroup))]
    [UpdateBefore(typeof(GameLoopStatesInitialisationGroup))]
    public partial class GameLoopDatabasesInitialisationGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(GameLoopSystemGroup))]
    [UpdateBefore(typeof(GameLoopInputGroup))]
    public partial class GameLoopStatesInitialisationGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(GameLoopSystemGroup))]
    [UpdateBefore(typeof(GameLoopPostInputGroup))]
    public partial class GameLoopInputGroup : ComponentSystemGroup { } // Non physics related inputs

    [UpdateInGroup(typeof(GameLoopSystemGroup))]
    [UpdateBefore(typeof(GameLoopPreGameplayGroup))]
    public partial class GameLoopPostInputGroup : ComponentSystemGroup { } // Consumation of inputs state to prepare gameplay state

    /*
    [UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
    public partial class GameLoopPhysicsInputGroup : ComponentSystemGroup { } // Raycast & co
    */

    [UpdateInGroup(typeof(GameLoopSystemGroup))]
    [UpdateBefore(typeof(GameLoopGameplayGroup))]
    public partial class GameLoopPreGameplayGroup : ComponentSystemGroup { } // Everything else

    [UpdateInGroup(typeof(GameLoopSystemGroup))]
    public partial class GameLoopGameplayGroup : ComponentSystemGroup { } // Everything else


    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class GameLoopPresentationGroup : ComponentSystemGroup { } // UI & stuff
}