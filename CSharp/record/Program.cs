using System;

// This file demonstrates C# `record` types with fully commented examples.
// It covers:
// - Purpose of records
// - When to use and when to avoid
// - Examples: record class, record struct, positional records, with-expressions,
//   inheritance, deconstruction, and equality semantics
// - Memory/allocation implications

// Quick summary:
// - `record` is a reference or value type with value-based equality semantics
//   (by default `record class` is reference type with compiler-generated equality and immutability-friendly features).
// - Records are ideal for immutable data carriers (DTOs, messages, snapshots),
//   where structural equality and concise syntax are useful.

// Example 1: simple positional record (immutable by default semantics)
// Positional records automatically generate properties, constructor, Deconstruct, and equality members.
// Person is a reference type (record class) and instances are heap-allocated.
public record Person(string FirstName, string LastName);

// Example 2: record with init-only properties and additional logic
public record Product
{
	public string Id { get; init; }
	public string Name { get; init; }
	public decimal Price { get; init; }

	// You can add computed properties or methods
	public decimal PriceWithTax(decimal taxRate) => Price * (1 + taxRate);
}

// Example 3: record inheritance and `with` expressions
public record Employee(string FirstName, string LastName, string Role) : Person(FirstName, LastName)
{
	public string EmployeeId { get; init; } = Guid.NewGuid().ToString("N");
}

// Example 4: record struct (value-type record introduced in C# 10)
// Record structs have value-type semantics (stack or inline in other objects).
// They also get generated equality and `with` support.
// Instances of Point2D behave like structs (no heap allocation unless boxed).
public readonly record struct Point2D(double X, double Y);

class Program
{
	static void Main()
	{
		Console.WriteLine("--- Record examples and guidance ---\n");

		// Purpose and typical use cases
		Console.WriteLine("Purpose: concise immutable data carriers with structural equality");
		Console.WriteLine("Scenarios to use: DTOs, messages, config values, small immutable snapshots\n");

		// When not to use records
		Console.WriteLine("Scenarios to avoid records:");
		Console.WriteLine("- Large mutable objects with complex identity semantics (prefer classes).");
		Console.WriteLine("- Types requiring reference equality semantics (object identity) for business logic.");
		Console.WriteLine("- When the type will be frequently mutated in-place (records favor immutability).\n");

		// Using positional record
		var p1 = new Person("Jane", "Doe");
		var p2 = new Person("Jane", "Doe");
		Console.WriteLine($"p1 == p2? {p1 == p2} (structural equality)");
		Console.WriteLine();

		// with-expression creates a shallow copy with modifications (for reference records)
		var p3 = p1 with { FirstName = "Janet" };
		Console.WriteLine($"p1: {p1}, p3 (with): {p3}");
		Console.WriteLine();

		// record class with init-only properties
		var prod = new Product { Id = "P1", Name = "Widget", Price = 9.99m };
		Console.WriteLine($"Product: {prod.Name} costs {prod.Price}");
		Console.WriteLine($"Price with tax: {prod.PriceWithTax(0.08m):C}");
		Console.WriteLine();

		// record inheritance and generated equality
		var e1 = new Employee("Alice", "Smith", "Engineer") { EmployeeId = "E-100" };
		var e2 = e1 with { Role = "Senior Engineer" };
		Console.WriteLine($"e1: {e1}");
		Console.WriteLine($"e2 (with new role): {e2}");
		Console.WriteLine();

		// record struct: value-type semantics with generated equality
		var a = new Point2D(1, 2);
		var b = new Point2D(1, 2);
		Console.WriteLine($"Point2D equality (value): {a.Equals(b)}");
		Console.WriteLine();

		// Deconstruction
		var (first, last) = p1;
		Console.WriteLine($"Deconstructed person: {first} {last}");
		Console.WriteLine();

		// Memory / Allocation notes
		Console.WriteLine("Memory / Allocation notes:");
		Console.WriteLine("- `record class` is a reference type: instances are allocated on the heap like normal classes.");
		Console.WriteLine("- `record struct` is a value type: instances behave like structs (stack or inline in other objects), so copying cost applies.");
		Console.WriteLine("- `with` expressions create a shallow copy: reference fields remain shared unless you perform deep clones.");
		Console.WriteLine("- Structural equality (value-based) compares by generated members; this can be more expensive than reference equality for large objects.");
		Console.WriteLine("- Prefer small, immutable data shapes for records to keep copy and equality cost low.\n");

		// Practical tips
		Console.WriteLine("Practical tips:");
		Console.WriteLine("- Use records for immutable DTOs, messages, and simple state snapshots.");
		Console.WriteLine("- Use record structs for small value types where you want generated equality and concise syntax.");
		Console.WriteLine("- Avoid exposing mutable reference fields from records; prefer init-only or private setters.");
		Console.WriteLine();

		Console.WriteLine("All record examples complete.");
	}
}

