namespace Commons.ServicesLocator {
    public interface IServiceBase {
        public void Initialize();

        public void PostInit() {} // Optional Function could be overridden 

        public void Destroy();
    }
}