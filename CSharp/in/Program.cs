using System;
using System.Linq;

namespace InKeywordExamples
{
	struct BigStruct
	{
		public long A, B, C, D;
		public BigStruct(long a, long b, long c, long d) { A = a; B = b; C = c; D = d; }
		public override string ToString() => $"A={A},B={B},C={C},D={D}";
	}

	// Example of generic contravariance with 'in' on type parameter
	interface IConsumer<in T>
	{
		void Consume(T item);
	}

	class Animal { public string Name => "animal"; }
	class Cat : Animal { public string Meow => "meow"; }

	class AnimalConsumer : IConsumer<Animal>
	{
		public void Consume(Animal a) => Console.WriteLine($"Consumed an animal: {a.Name}");
	}

	class Program
	{
		// 'in' parameter modifier — pass by reference, readonly inside the callee
		static void PrintIn(in BigStruct s)
		{
			// s is read-only here; attempting to assign to s.A would be a compile error
			Console.WriteLine($"PrintIn received (readonly): {s}");
		}

		// 'ref' example to contrast with 'in'
		static void ModifyRef(ref BigStruct s)
		{
			s.A = 999; // mutates the caller's storage
		}

		static void Main()
		{
			Console.WriteLine("1) 'in' used in foreach / query expressions:");
			var numbers = new[] { 1, 2, 3, 4 };
			foreach (var n in numbers)
				Console.WriteLine($" - {n}");

			var evens = from n in numbers
						where n % 2 == 0
						select n;
			Console.WriteLine("Evens from LINQ query:");
			foreach (var e in evens)
				Console.WriteLine($" > {e}");

			Console.WriteLine();
			Console.WriteLine("2) 'in' as a parameter modifier (ref readonly):");
			var big = new BigStruct(1, 2, 3, 4);
			// Explicit 'in' call
			PrintIn(in big);
			// Implicit 'in' (compiler will infer and treat it as readonly ref)
			PrintIn(big);
			// You may pass a temporary to an 'in' parameter
			PrintIn(new BigStruct(10, 20, 30, 40));

			Console.WriteLine();
			Console.WriteLine("Contrast with 'ref' (call requires a variable):");
			ModifyRef(ref big);
			Console.WriteLine($"After ModifyRef: {big}");

			Console.WriteLine();
			Console.WriteLine("3) 'in' on a generic type parameter (contravariance):");
			IConsumer<Animal> animalConsumer = new AnimalConsumer();
			// Because the type parameter is declared 'in', we can assign IConsumer<Animal> to IConsumer<Cat>
			IConsumer<Cat> catConsumer = animalConsumer; // contravariance
			catConsumer.Consume(new Cat());

			Console.WriteLine();
			Console.WriteLine("Done.");
		}
	}
}
