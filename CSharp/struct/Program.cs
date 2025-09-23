using System;
using System.Runtime.InteropServices;

// This file demonstrates practical usage of C# structs (value types) and
// explains memory allocation implications: stack vs heap, copying, boxing,
// ref semantics, readonly structs, and ref structs (stack-only types).

// large structs should generally be avoided as method parameters/returns because each copy is more expensive.


class Program
{
    static void Main()
    {
        Console.WriteLine("--- Struct examples and memory notes ---\n");

        // 1) Basic struct: value-type semantics (copy on assignment)
        Console.WriteLine("1) Basic struct copy behavior:");
        Point p1 = new Point { X = 1, Y = 2 }; // struct initialized (no heap allocation)
        Point p2 = p1;                          // copy: p2 is a separate value
        p2.X = 10;                              // modifying p2 does NOT change p1
        Console.WriteLine($"p1 = {p1}, p2 = {p2}");
        Console.WriteLine("Explanation: p1 and p2 are independent copies (stack or inline in containing object).\n");

        // 2) Default values for structs
        Console.WriteLine("2) Default struct values:");
        Point defaultPoint = default;           // all fields zeroed
        Console.WriteLine($"defaultPoint = {defaultPoint}\n"); // what this \n does is add a newline after printing the defaultPoint

        // 3) Immutable struct: prefer readonly struct for safety and performance
        Console.WriteLine("3) Immutable readonly struct:");
        ImmutablePoint ip = new ImmutablePoint(3, 4);
        Console.WriteLine($"ImmutablePoint: {ip}");
        Console.WriteLine("readonly struct signals immutability and enables compiler optimizations.\n");

        // 4) Size considerations: small structs can be efficient; large structs are expensive to copy
        Console.WriteLine("4) Size of structs (bytes):");
        Console.WriteLine($"SizeOf(SmallStruct) = {Marshal.SizeOf<SmallStruct>()} bytes");
        Console.WriteLine($"SizeOf(LargeStruct) = {Marshal.SizeOf<LargeStruct>()} bytes");
        Console.WriteLine("Large structs should generally be avoided as method parameters/returns because each copy is more expensive.\n");

        // 5) Boxing: when a value type is converted to object or an interface, it gets allocated on the heap
        Console.WriteLine("5) Boxing / Unboxing:");
        Point pBox = new Point { X = 7, Y = 8 };
        object boxed = pBox;                    // boxing: a heap allocation that copies the value
        Console.WriteLine($"boxed (ToString) = {boxed}");
        // Modify original value - boxed copy is independent
        pBox.X = 100;
        Console.WriteLine($"original after change = {pBox}, boxed still = {boxed}");
        // Unbox back to value type
        Point unboxed = (Point)boxed;
        Console.WriteLine($"unboxed = {unboxed}\n");
        // Note: modifying 'boxed' directly is not possible since it's of type 'object'
        unboxed.X = 200; // modifies unboxed copy, not boxed or original
        Console.WriteLine($"after modifying unboxed: unboxed = {unboxed}, boxed = {boxed}, original pBox = {pBox}\n");

        // 6) Mutating structs inside arrays: use ref to avoid copying
        Console.WriteLine("6) Ref locals to avoid copies when mutating array elements:");
        Point[] arr = new Point[2];
        arr[0] = new Point { X = 1, Y = 1 };
        ref Point r = ref arr[0];               // reference to the array element (no copy)
        r.X = 42;                               // modifies the array element in-place
        Console.WriteLine($"arr[0] after ref mutation = {arr[0]}\n");

        // 7) 'in' parameters and readonly struct: avoid defensive copies
        Console.WriteLine("7) Passing readonly struct by 'in' to avoid copy:");
        LargeStruct large = LargeStruct.Create();
        PrintLargeIn(in large);     // passed by reference read-only -> avoids copy
                                    // PrintLarge(large);      // would copy the whole struct
        Console.WriteLine();

        // 8) ref struct example (stack-only types, cannot be boxed)
        Console.WriteLine("8) ref struct (stack-only) example using Span<T>:");
        Span<byte> buffer = stackalloc byte[4]; // stack allocation
        buffer[0] = 1; buffer[1] = 2; buffer[2] = 3; buffer[3] = 4;
        SumSpan(stackalloc byte[] { 10, 20, 30 }); // passing a stackalloc span
        Console.WriteLine();

        // 9) When to choose struct vs class (summary)
        Console.WriteLine("9) Guideline summary:");
        Console.WriteLine("- Use a struct for small (typically <= 16 bytes), immutable, logically-value types (Point, Color, Complex).");
        Console.WriteLine("- Use a class for large, mutable, or reference-equality types and when polymorphism is required.");
        Console.WriteLine("- Beware of boxing, large-copy overhead, and mutability pitfalls.\n");
    }

    // A small struct (good candidate)
    struct Point
    {
        // public fields for simplicity in examples (properties are fine too)
        public int X;
        public int Y;
        public override string ToString() => $"Point(X={X}, Y={Y})";
    }

    // Immutable struct: mark 'readonly' to indicate immutability and enable optimizations.
    readonly struct ImmutablePoint
    {
        public readonly int X;
        public readonly int Y;

        public ImmutablePoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"ImmutablePoint(X={X}, Y={Y})";
    }

    // SmallStruct and LargeStruct for size demonstration
    [StructLayout(LayoutKind.Sequential)]
    struct SmallStruct
    {
        public int A;   // 4 bytes
        public int B;   // 4 bytes -> typical size 8
    }

    [StructLayout(LayoutKind.Sequential)] // ensure predictable layout. this attributes is optional and means the same as default for structs but is added here for clarity
    struct LargeStruct
    {
        // several fields to make the struct large
        public long A;
        public long B;
        public long C;
        public long D;
        // typical size 32 bytes on 64-bit
        public static LargeStruct Create() => new LargeStruct { A = 1, B = 2, C = 3, D = 4 };
    }

    // Passing LargeStruct by value (would copy) vs by 'in' (no copy, read-only reference)
    static void PrintLarge(LargeStruct s)
    {
        // This method receives a copy of the struct -> copy cost
        Console.WriteLine($"PrintLarge received A={s.A}, size might have been copied.");
    }

    static void PrintLargeIn(in LargeStruct s) // what if i use 'ref' instead of 'in' here?
    // if ref is used instead of in, the method could modify the struct, which may not be desired
    // using 'in' ensures the method cannot modify the struct, providing safety and clarity of intent
    {
        // 'in' parameter passes a readonly reference -> avoids copying large struct
        Console.WriteLine($"PrintLargeIn received A={s.A}, passed by readonly reference (no copy).");
    }

    // Example consuming a ref struct: cannot be boxed or stored on the heap
    ref struct StackOnly
    {
        public Span<byte> Data;
        public StackOnly(Span<byte> data) => Data = data;
        public int Sum()
        {
            int sum = 0;
            foreach (var b in Data) sum += b;
            return sum;
        }
    }

    static void SumSpan(Span<byte> s)
    {
        var so = new StackOnly(s);
        Console.WriteLine($"Sum of span = {so.Sum()}");
    }
}

