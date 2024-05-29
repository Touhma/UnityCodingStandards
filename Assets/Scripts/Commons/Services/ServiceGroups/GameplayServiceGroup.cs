namespace Commons.Services.ServiceGroups
{
    [InitializeBefore(typeof(GameplayServiceGroup))]
    public class SettingsServiceGroup : IServiceGroup
    {
    }

    public class GameplayServiceGroup : IServiceGroup
    {
    }
}