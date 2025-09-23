// See https://aka.ms/new-console-template for more information
using System;

class Program
{
	static void Main()
	{
		Console.WriteLine("stackalloc examples\n");

		SafeStackAllocExample();
		Console.WriteLine();

		UnsafeStackAllocExample();
		Console.WriteLine();

		Console.WriteLine("Examples complete.");
	}

	// Example 1: safe form that returns a Span<T> (no unsafe required)
	// - Allocates on the current thread's stack
	// - Lifetime is limited to this method/scope
	// - Good for small, short-lived buffers
	static void SafeStackAllocExample()
	{
		Console.WriteLine("Safe stackalloc (Span<byte>) demo");

		// Allocate 1024 bytes on the stack. This does not create a heap allocation
		Span<byte> buffer = stackalloc byte[1024];

        // teste paulo
        // stackalloc can only allocate unmanaged value types on the stack.
        // Span<string> buffer2 = stackalloc string[1024];
        Span<string> buffer2 = new string[1024];
        string[] buffer3 = new string[1024];

        // Fill with a repeating pattern (0..255)
        for (int i = 0; i < buffer.Length; i++)
            buffer[i] = (byte)(i & 0xFF);

		// Use values so the JIT does not optimize the buffer away
		Console.WriteLine($"  buffer[0] = {buffer[0]}, buffer[512] = {buffer[512]}");
	}

	// Example 2: unsafe pointer form (requires 'unsafe' context)
	// - Returns a pointer to stack memory (T*)
	// - Still allocated on the current thread's stack and popped at method return
	// - Useful in low-level interop scenarios or performance-critical code
	unsafe static void UnsafeStackAllocExample()
	{
		Console.WriteLine("Unsafe stackalloc (byte*) demo");

		// Allocate 256 bytes on the stack and get a byte* to it
		byte* p = stackalloc byte[256];

		// Write some values
		for (int i = 0; i < 256; i++)
			p[i] = (byte)i; // values 0..255

		// Read back a couple of values
		Console.WriteLine($"  p[0] = {p[0]}, p[200] = {p[200]}");
	}
}
