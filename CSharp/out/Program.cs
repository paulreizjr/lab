using System;
using System.Collections.Generic;

namespace OutKeywordExamples
{
	class Animal { public string Name { get; set; } = "animal"; }
	class Dog : Animal { public string Bark => "woof"; }

	// Covariant producer: 'out T' allows T to be used in output positions
	interface IProducer<out T>
	{
		T Produce();
	}

	class DogProducer : IProducer<Dog>
	{
		public Dog Produce() => new Dog { Name = "Rex" };
	}

	class Program
	{
		// Example of 'out' parameters: the Try-style pattern
		static bool TryParsePoint(string s, out int x, out int y)
		{
			x = 0; y = 0;
			if (string.IsNullOrWhiteSpace(s)) return false;
			var parts = s.Split(',');
			if (parts.Length != 2) return false;
			if (!int.TryParse(parts[0].Trim(), out x)) return false;
			if (!int.TryParse(parts[1].Trim(), out y)) return false;
			return true;
		}

		static void FillTwo(out int a, out int b)
		{
			a = 1; b = 2; // out parameters must be assigned before returning
		}

		static void Main()
		{
			Console.WriteLine("1) 'out' parameter modifier (Try pattern):");
			if (TryParsePoint("10, 20", out int px, out int py))
			{
				Console.WriteLine($"Parsed point: x={px}, y={py}");
			}
			else
			{
				Console.WriteLine("Failed to parse point");
			}

			Console.WriteLine();
			Console.WriteLine("2) Inline declaration with 'out var' and discard with 'out _':");
			string maybeNumber = "42";
			if (int.TryParse(maybeNumber, out var number))
			{
				Console.WriteLine($"Parsed number via out var: {number}");
			}

			// Discard example: we only care whether parse succeeded
			if (int.TryParse("notANumber", out _))
			{
				Console.WriteLine("This won't run");
			}
			else
			{
				Console.WriteLine("Discard used: parse failed but we don't keep the value");
			}

			Console.WriteLine();
			Console.WriteLine("3) 'out' on generic type parameters (covariance):");
			IProducer<Dog> dogProducer = new DogProducer();
			// Because IProducer<T> declares 'out T', it's covariant: IProducer<Dog> -> IProducer<Animal>
			IProducer<Animal> animalProducer = dogProducer;
			Animal a = animalProducer.Produce();
			Console.WriteLine($"Produced animal name: {a.Name} (runtime type: {a.GetType().Name})");

			Console.WriteLine();
			Console.WriteLine("4) 'out' parameter rules and edge-cases:");
			FillTwo(out int a1, out int b1); // out requires assignment inside callee
			Console.WriteLine($"FillTwo produced: a1={a1}, b1={b1}");

			// The following would be a compile-time error if uncommented: using an out variable before assignment
			int z; if (int.TryParse("z", out z)) Console.WriteLine(z); // must be definitely assigned before use

			Console.WriteLine();
			Console.WriteLine("Done.");
		}
	}
}
