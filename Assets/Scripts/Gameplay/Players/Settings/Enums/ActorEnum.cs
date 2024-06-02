using Commons.Architectures;

namespace Players.Settings.Enums {
    [GenerateComponentTags]
    public enum Actor : byte
    {
        // Logistic
        Conveyor = 1,
        Splitter = 2,
        Merger = 3,

        //Processing
        Miner = 20,
        Smelter = 21,
        Processor = 22,

        //Storage,
        Container = 40,

        //Socket Support
        ConveyorPole = 60,
    }
    
    [GenerateComponentTags]
    public enum Testing : byte
    {
        // Logistic
        Conveyor = 1,
        Splitter = 2,
        Merger = 3,

        //Processing
        Miner = 20,
        Smelter = 21,
        Processor = 22,

        //Storage,
        Container = 40,

        //Socket Support
        ConveyorPole = 60,
    }

    public struct test {
        private ActorConveyorTag top;

    }
    
    
}