using System;

// Examples showing the 'override' keyword in C#.
// - 'virtual' marks a base member as overridable.
// - 'override' replaces a virtual/abstract member in a derived type.
// - 'sealed override' prevents further overrides.
// - abstract members must be overridden in concrete subclasses.

namespace OverrideExamples
{
	// Virtual method example
	class Animal
	{
		public virtual string Speak() => "(silence)";
		public override string ToString() => "Animal";
	}

	class Dog : Animal
	{
		public override string Speak() => "Woof";
		public override string ToString() => "Dog";
	}

	// Sealed override example - prevents further overriding of Speak()
	class LoudDog : Dog
	{
		public sealed override string Speak() => "WOOF!!!";
	}

	// Abstract member example
	abstract class Shape
	{
		public abstract double Area();
	}

	class Circle : Shape
	{
		public double Radius { get; }
		public Circle(double radius) => Radius = radius;
		public override double Area() => Math.PI * Radius * Radius;
	}

	// Virtual method overridden in a subclass
	class Vehicle
	{
		public virtual void Drive() => Console.WriteLine("Vehicle driving (base)");
	}

	class Car : Vehicle
	{
		public override void Drive() => Console.WriteLine("Car driving (overridden)");
	}

	class Program
	{
		static void Main()
		{
			Console.WriteLine("1) Basic override and polymorphism:");
			Animal a = new Animal();
			Animal d = new Dog();
			Animal ld = new LoudDog();
			Console.WriteLine($"Animal says: {a.Speak()}");
			Console.WriteLine($"Dog says (via Animal reference): {d.Speak()}");
			Console.WriteLine($"LoudDog says (sealed override): {ld.Speak()}");
			Console.WriteLine($"ToString override: {d}");

			Console.WriteLine();
			Console.WriteLine("2) Abstract member override:");
			Shape c = new Circle(2.5);
			Console.WriteLine($"Circle area: {c.Area():F3}");

			Console.WriteLine();
			Console.WriteLine("3) Virtual method override (different hierarchy):");
			Vehicle v = new Car();
			v.Drive();

			Console.WriteLine();
			Console.WriteLine("4) Notes and edge-cases:");
			Console.WriteLine(" - You can only override members declared virtual, abstract, or already override in the base.");
			Console.WriteLine(" - 'sealed override' stops further derived types from overriding that member.");
			Console.WriteLine(" - Overriding is runtime polymorphism: behavior depends on the runtime type, not the compile-time type.");

			// The following commented code would not compile:
			// class Bad : Animal { public override int Speak() => 42; } // signature mismatch
			// class TryOverride : LoudDog { public override string Speak() => "hi"; } // can't override sealed member

			Console.WriteLine();
			Console.WriteLine("Done.");
		}
	}
}
