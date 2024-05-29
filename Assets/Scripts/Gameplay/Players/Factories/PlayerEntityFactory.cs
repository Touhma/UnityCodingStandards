using Players.Components;
using Commons.Architectures;

namespace Players.Factories {
    // This will create a UnmanagedSetting shared static & it's shorcut so you can call : EntityFactory.PlayerEntityFactory directly
    [EntityFactory]
    // this is the list of all the components you want your entity to have
    [GenWith(typeof(PositionComponent), typeof(HealthComponent), typeof(PlayerStateEnabled), typeof(PlayerTag))]
    public partial struct PlayerEntityFactory { }
}