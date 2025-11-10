// See https://aka.ms/new-console-template for more information
using ClassLibrary1;
using Microsoft.Extensions.DependencyInjection;

IServiceCollection serviceCollection = new ServiceCollection();

OutDoor outDoor = new OutDoor();
outDoor.AddReference(serviceCollection);

outDoor.PrintMyProperty();
outDoor.ConfigureMyProperty(stupidClass => stupidClass.MyProperty = "This is a stupid property");
outDoor.PrintMyProperty();

var serviceProvider = serviceCollection.BuildServiceProvider();

var stupidInstance = serviceProvider.GetRequiredService<IStupidInterface>();

Console.WriteLine(stupidInstance.Hi());
