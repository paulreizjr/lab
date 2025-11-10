using Microsoft.Extensions.DependencyInjection;

namespace ClassLibrary1
{
    public class OutDoor
    {
        private StupidClass? MyStupidClass { get; set; }

        public void AddReference(IServiceCollection serviceContainer)
        {
            serviceContainer.AddScoped<IStupidInterface, StupidClass>();
        }

        public void ConfigureMyProperty(Action<StupidClass> configure)
        {
            configure(MyStupidClass ??= new StupidClass());
        }

        public void PrintMyProperty()
        {
            if (MyStupidClass != null)
            {
                Console.WriteLine($"MyProperty: {MyStupidClass.MyProperty}");
            }
            else
            {
                Console.WriteLine("MyStupidClass is not configured.");
            }
        }
    }
}
