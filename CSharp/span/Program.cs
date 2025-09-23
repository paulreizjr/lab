// See https://aka.ms/new-console-template for more information
using System;
using System.Buffers;
using System.Text;

// This file demonstrates `Span<T>`, `ReadOnlySpan<T>`, `Memory<T>`, `stackalloc`,
// and practical memory allocation considerations in C#.
// Key points:
// - `Span<T>` is a stack-only type that provides a safe window over contiguous memory.
// - `Memory<T>` is the heap-allocatable counterpart that can be stored and used across async/await.
// - `stackalloc` allocates memory on the stack (fast, short-lived).
// - Avoid boxing or storing a `Span<T>` on the heap; use `Memory<T>` when you need persistence.

class Program
{
	static unsafe void Main()
	{
		Console.WriteLine("--- Span and Memory examples ---\n");

		// 1) Basic Span over existing array (no allocation)
		Console.WriteLine("1) Span over array (no allocation):");
		int[] arr = { 1, 2, 3, 4, 5 };
		Span<int> span = arr; // implicit conversion from array to Span<T>
		span[1] = 20; // modifies underlying array
		Console.WriteLine($"arr after modification: {string.Join(", ", arr)}");
		Console.WriteLine();

		// 2) ReadOnlySpan for read-only view (zero allocation)
		Console.WriteLine("2) ReadOnlySpan for safe read-only access:");
		ReadOnlySpan<int> ro = arr;
		Console.WriteLine($"Sum via ReadOnlySpan: {Sum(ro)}");
		Console.WriteLine();

		// 3) stackalloc: stack-allocated memory for short-lived buffers
		Console.WriteLine("3) stackalloc (stack-allocated memory):");
		Span<byte> buffer = stackalloc byte[16]; // allocation on the stack
		Encoding.UTF8.GetBytes("hello", buffer);
		var s = Encoding.UTF8.GetString(buffer[..5]); // [..5] means slice first 5 bytes
                                                      // Note: `stackalloc` memory is automatically freed when the method returns
		Console.WriteLine($"Read from stackalloc buffer: {s}");
		Console.WriteLine();

		// 4) Memory<T> for heap-persisted memory (can be stored and used with async)
		Console.WriteLine("4) Memory<T> (heap-friendly, can be used across awaits):");
		Memory<int> mem = new int[] { 10, 20, 30, 40 };
		var memSpan = mem.Span; // get a Span<T> to access data
		memSpan[0] = 99;
		Console.WriteLine($"Memory contents: {string.Join(", ", mem.ToArray())}");
		Console.WriteLine();

        // 5) Using ArrayPool<T> to rent buffers and avoid allocations
        // Great for large or frequently used buffers
        // that would otherwise pressure the GC.
		Console.WriteLine("5) ArrayPool<T> to reuse buffers and reduce GC pressure:");
		var pool = ArrayPool<byte>.Shared;
		byte[] rented = pool.Rent(256); // large buffer rented from pool
		try
		{
			Span<byte> rentedSpan = rented;
			// use rentedSpan without allocating new arrays
			rentedSpan[0] = 1;
			Console.WriteLine($"Rented buffer first byte: {rentedSpan[0]}");
		}
		finally
		{
			pool.Return(rented);
		}
		Console.WriteLine();

        // 6) Pinning for interop: Get a pointer safely to managed heap memory
        // Note: pinning a heap array is common for native interop. Avoid pinning for long durations.
        // native interop scenarios often require fixed pointers to managed memory.
        // native interop means calling into unmanaged code (C/C++ libraries) from managed C# code.
		Console.WriteLine("6) Pinning memory for native interop (fixed on heap array):");
		int[] heap = { 7, 8, 9 };
		unsafe
		{
			fixed (int* p = heap)
			{
				Console.WriteLine($"First element via pointer: {p[0]}");
			}
		}
		Console.WriteLine();

		// 7) Avoid capturing Span<T> in lambdas that escape
		Console.WriteLine("7) Span lifetime rules (don’t store Span on heap):");
		var localSpan = stackalloc int[3] { 1, 2, 3 };
		// The following would be illegal: Func<Span<int>> f = () => localSpan;
		Console.WriteLine("You cannot return or store a Span<T> that points to stackalloc beyond its scope.");
		Console.WriteLine();

		// 8) Simple text parsing example using ReadOnlySpan<char>
		Console.WriteLine("8) Parse CSV line with ReadOnlySpan<char> (no allocations):");
		var csv = "100,hello,3.14";
		ParseCsvLine(csv.AsSpan());
		Console.WriteLine();

		Console.WriteLine("All Span/Memory examples complete.");
	}

	static int Sum(ReadOnlySpan<int> values)
	{
		int sum = 0;
		foreach (var v in values) sum += v;
		return sum;
	}

	static void ParseCsvLine(ReadOnlySpan<char> line)
	{
		// Parse simple csv: id,name,value
		var first = line.Slice(0, line.IndexOf(','));
		var rest = line.Slice(first.Length + 1);
		var second = rest.Slice(0, rest.IndexOf(','));
		var third = rest.Slice(second.Length + 1);

		Console.WriteLine($"id: {int.Parse(first)}, name: {new string(second)}, value: {double.Parse(third)}");
		// Note: `new string(ReadOnlySpan<char>)` allocates; you can avoid allocations further
		// by parsing values directly from spans when supported.
	}
}

