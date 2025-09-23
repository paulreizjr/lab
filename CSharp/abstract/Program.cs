// Examples demonstrating the use of the `abstract` keyword in C#.
// This file contains several short examples showing:
// - abstract classes and abstract members
// - abstract properties and methods
// - providing common implementations in abstract classes
// - sealed and virtual interactions
// - abstract generic base classes
// - abstract constructors behavior (note: constructors themselves cannot be abstract)
// To try the examples: `dotnet run` in the `abstract` project folder.

using System;

// Example 1: Simple abstract class with abstract method
// An abstract class cannot be instantiated directly. It can contain
// abstract members (which have no implementation) and non-abstract members
// (with implementations) that derived classes inherit.
abstract class Animal
{
	// Abstract method: derived classes must provide an implementation.
	public abstract void Speak();

	// Non-abstract method: derived classes inherit this implementation.
	public void Sleep()
	{
		Console.WriteLine("The animal sleeps.");
	}
}

// Concrete class implementing the abstract method
class Dog : Animal
{
	public override void Speak()
	{
		Console.WriteLine("Woof!");
	}
}

// Example 2: Abstract properties and protected helper methods
abstract class Shape
{
	// Abstract property: derived classes must implement get (and optionally set).
	public abstract double Area { get; }

	// A protected method that derived classes can use to validate dimensions.
	protected void ValidatePositive(double value, string name)
	{
		if (value <= 0)
			throw new ArgumentOutOfRangeException(name, "Value must be positive.");
	}
}

class Circle : Shape
{
	public double Radius { get; }

	public Circle(double radius)
	{
		ValidatePositive(radius, nameof(radius));
		Radius = radius;
	}

	// Implement the abstract property
	public override double Area => Math.PI * Radius * Radius;
}

// Example 3: Abstract generic base class
abstract class Repository<T>
{
	// Abstract CRUD-like method: derived repositories implement storage details.
	public abstract void Add(T item);

	// Virtual method provides a default behavior that derived classes can override.
	public virtual void LogAction(string action)
	{
		Console.WriteLine($"Repository action: {action}");
	}
}

class InMemoryRepository<T> : Repository<T>
{
	private readonly System.Collections.Generic.List<T> _items = new();

	public override void Add(T item)
	{
		_items.Add(item);
		LogAction($"Added item of type {typeof(T).Name}");
	}

	// Optionally override the virtual method
	public override void LogAction(string action)
	{
		// Call base for consistency, then add more info
		base.LogAction(action);
		Console.WriteLine($"Current count: {_items.Count}");
	}
}

// Example 4: Abstract class with sealed override
// You can prevent further overrides by sealing a method in a derived class.
abstract class Logger
{
	public abstract void Write(string message);
}

class BaseLogger : Logger
{
	public sealed override void Write(string message)
	{
		// Sealed - no further derived class can override this method.
		Console.WriteLine($"BaseLogger: {message}");
	}
}

class SpecialLogger : BaseLogger
{
	// The following would be illegal because Write is sealed in BaseLogger:
	// public override void Write(string message) { ... }
}

// Note on abstract constructors:
// Constructors themselves cannot be abstract. Abstract classes can define
// constructors which are executed when derived types are constructed.
abstract class AbstractWithCtor
{
	protected AbstractWithCtor()
	{
		Console.WriteLine("AbstractWithCtor constructor running.");
	}
}

class DerivedWithCtor : AbstractWithCtor
{
	public DerivedWithCtor()
	{
		Console.WriteLine("DerivedWithCtor constructor running.");
	}
}

// Main program demonstrating usage
class Program
{
	static void Main()
	{
		Console.WriteLine("--- Abstract class: Animal/Dog ---");
		Animal myDog = new Dog();
		myDog.Speak();    // Woof!
		myDog.Sleep();    // The animal sleeps.

		Console.WriteLine();
		Console.WriteLine("--- Abstract property: Shape/Circle ---");
		Shape c = new Circle(2.5);
		Console.WriteLine($"Circle radius: {((Circle)c).Radius}, Area: {c.Area:F2}");

		Console.WriteLine();
		Console.WriteLine("--- Abstract generic: Repository<T> ---");
		var repo = new InMemoryRepository<string>();
		repo.Add("hello");
		repo.Add("world");

		Console.WriteLine();
		Console.WriteLine("--- Sealed override example ---");
		Logger logger = new BaseLogger();
		logger.Write("This is a test");

		Console.WriteLine();
		Console.WriteLine("--- Abstract constructor demonstration ---");
		var derived = new DerivedWithCtor();

		Console.WriteLine();
		Console.WriteLine("All basic examples complete.");

		Console.WriteLine();
		Console.WriteLine("--- Additional: Abstract events ---");
		var publisher = new ButtonPublisher();
		var subscriber = new ClickSubscriber();
		// Subscribe to the abstract event (concrete implementation raises it)
		subscriber.Subscribe(publisher);
		publisher.SimulateClick();

		Console.WriteLine();
		Console.WriteLine("--- Additional: Interfaces vs Abstract Classes ---");
		IAnimal iDog = new DogInterfaceAdapter();
		iDog.Speak();

		Console.WriteLine();
		Console.WriteLine("--- Additional: Abstract nested types ---");
		var container = new Container.ConcreteNested();
		container.Run();

		Console.WriteLine();
		Console.WriteLine("All examples complete.");
	}
}

// ----------------------- Additional Examples -----------------------

// 1) Abstract events
// Abstract classes can declare abstract events that derived classes must
// implement (typically by declaring add/remove accessors or by exposing
// a normal event field). Here we show a publisher that exposes an abstract
// event and a concrete class that raises it.
abstract class ButtonBase
{
	// Abstract event: derived classes must provide the event implementation.
	public abstract event EventHandler? Clicked;

	// Non-abstract helper method to simulate a click in derived classes.
	protected void OnClicked()
	{
		// Derived class will invoke the concrete event field/accessor.
	}
}

class ButtonPublisher : ButtonBase
{
	// Backing event field for the abstract event
	private event EventHandler? _clicked;

	// Implement the abstract event by forwarding add/remove to the field.
	public override event EventHandler? Clicked
	{
		add { _clicked += value; }
		remove { _clicked -= value; }
	}

	// Method to simulate a user clicking the button
	public void SimulateClick()
	{
		Console.WriteLine("ButtonPublisher: Simulating click...");
		_clicked?.Invoke(this, EventArgs.Empty);
	}
}

class ClickSubscriber
{
	public void HandleClick(object? sender, EventArgs e)
	{
		Console.WriteLine("ClickSubscriber: Received click event.");
	}

	public void Subscribe(ButtonBase button)
	{
		button.Clicked += HandleClick;
	}
}

// 2) Interfaces vs Abstract Classes
// Interfaces define a contract with no implementation (prior to default
// interface methods). Abstract classes can provide both contract and
// shared implementation. Use interfaces when you need multiple inheritance
// of behavior; use abstract classes when you want to share code/state.
interface IAnimal
{
	void Speak();
}

// Adapter that implements the interface by using an existing concrete class
class DogInterfaceAdapter : IAnimal
{
	private readonly Dog _dog = new();
	public void Speak() => _dog.Speak();
}

// 3) Abstract nested types
// Abstract classes can contain nested types (classes, interfaces, etc.).
// These nested types can be abstract as well. Here the outer class exposes
// an abstract nested class and a concrete nested implementation.
abstract class Container
{
	// Abstract nested class
	public abstract class NestedBase
	{
		public abstract void Run();
	}

	// Concrete nested class implementing the abstract nested class
	public class ConcreteNested : NestedBase
	{
		public override void Run()
		{
			Console.WriteLine("ConcreteNested.Run invoked.");
		}
	}
}
