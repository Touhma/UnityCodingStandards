namespace Commons.Services
{
    public interface IServiceBase
    {
        public void Initialize();

        public void PostInit() { }
        
        public void Destroy(){}
    }
}