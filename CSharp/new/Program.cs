using System;
using System.Collections.Generic;

namespace NewKeywordExamples
{
	// Simple class for object creation examples
	class Person
	{
		public string Name { get; }
		public Person(string name) => Name = name;
		public override string ToString() => $"Person(Name={Name})";
	}

	// For the generic new() constraint example
	class WithDefaultCtor
	{
		public int Value { get; set; } = 123;
		public override string ToString() => $"WithDefaultCtor(Value={Value})";
	}

	// Member hiding example
	class BaseClass
	{
		public void Show() => Console.WriteLine("BaseClass.Show");
		public virtual void VirtualShow() => Console.WriteLine("BaseClass.VirtualShow");
	}

	class DerivedClass : BaseClass
	{
		public new void Show() => Console.WriteLine("DerivedClass.Show (hidden)");
		public override void VirtualShow() => Console.WriteLine("DerivedClass.VirtualShow (overridden)");
	}

	struct S
	{
		public int X;
		public override string ToString() => $"S(X={X})";
	}

	class Factory<T> where T : new()
	{
		public T Create() => new T();
	}

	class Program
	{
		static void Main()
		{
			Console.WriteLine("=== 1) Object / array / anonymous-type creation ===");
			var p = new Person("Alice");
			Console.WriteLine(p);

			int[] a = new int[3];
			Console.WriteLine($"Array length: {a.Length}");

			var inferred = new[] { 1, 2, 3 };
			Console.WriteLine($"Inferred array: {string.Join(',', inferred)}");

			var anon = new { Name = "Bob", Age = 30 };
			Console.WriteLine($"Anonymous: Name={anon.Name}, Age={anon.Age}");

			Console.WriteLine();
			Console.WriteLine("=== 2) Target-typed new ===");
			Person p2 = new("Charlie");
			Console.WriteLine(p2);

			List<string> list = new();
			list.Add("one");
			Console.WriteLine($"List count: {list.Count}");

			Console.WriteLine();
			Console.WriteLine("=== 3) Generic new() constraint ===");
			var factory = new Factory<WithDefaultCtor>();
			var obj = factory.Create();
			Console.WriteLine(obj);

			Console.WriteLine();
			Console.WriteLine("=== 4) Member hiding with 'new' vs override ===");
			DerivedClass d = new DerivedClass();
			d.Show(); // calls derived Show
			((BaseClass)d).Show(); // calls base Show because it was hidden, not overridden
			d.VirtualShow(); // overridden method

			Console.WriteLine();
			Console.WriteLine("=== 5) new() for value types and parameterless construction ===");
			S s1 = new S();
			Console.WriteLine($"s1: {s1}");
			S s2 = new();
			Console.WriteLine($"s2: {s2}");

			Console.WriteLine();
			Console.WriteLine("Done.");
		}
	}
}
