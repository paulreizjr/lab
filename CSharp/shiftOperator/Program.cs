using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

/* 
 * SHIFT OPERATORS IN C#
 * 
 * PURPOSE:
 * - Bitwise shifting of binary representations left (<<) or right (>>)
 * - Left shift: multiplies by powers of 2 (x << n = x * 2^n)
 * - Right shift: divides by powers of 2 (x >> n = x / 2^n for positive numbers)
 * - Arithmetic right shift preserves sign bit for signed types
 * - Logical right shift fills with zeros (unsigned types)
 * 
 * SCENARIOS TO USE:
 * - Fast multiplication/division by powers of 2
 * - Bit manipulation and flag operations
 * - Hash function implementations
 * - Memory address calculations
 * - Performance-critical mathematical operations
 * - Graphics programming (pixel manipulation, color channel extraction)
 * - Cryptographic algorithms
 * - Data compression algorithms
 * - Network protocol implementations
 * 
 * SCENARIOS NOT TO USE:
 * - General arithmetic where readability matters more than performance
 * - When the shift count is not a compile-time constant (unpredictable performance)
 * - Business logic calculations where clarity is paramount
 * - When working with decimal or floating-point numbers
 * - Educational/learning code where explicit operations are clearer
 * 
 * MEMORY ALLOCATION:
 * - Shift operations are CPU instructions with no heap allocation
 * - No garbage collection pressure
 * - Stack-only operations for primitive types
 * - Constant time complexity O(1)
 * - Very cache-friendly operations
 * 
 * MULTITHREADING ASPECTS:
 * - Shift operations on local variables are inherently thread-safe
 * - Shared variables require synchronization like any other operation
 * - Atomic operations may be needed for shared counters/flags
 * - No special thread safety considerations beyond normal variable access
 * - Can be used safely in parallel algorithms for independent data
 */

namespace ShiftOperatorExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== C# Shift Operators Examples ===\n");

            BasicShiftOperations();
            ArithmeticVsLogicalShift();
            MultiplicationDivisionOptimization();
            BitManipulationTechniques();
            FlagOperations();
            HashingExample();
            ColorManipulation();
            PerformanceComparison();
            MultithreadingExample();
            EdgeCasesAndPitfalls();
            RealWorldApplications();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Demonstrates basic left and right shift operations
        /// </summary>
        static void BasicShiftOperations()
        {
            Console.WriteLine("1. Basic Shift Operations:");
            
            int value = 12; // Binary: 1100
            Console.WriteLine($"   Original value: {value} (Binary: {Convert.ToString(value, 2).PadLeft(8, '0')})");
            
            // Left shift - multiplies by 2^n
            int leftShift1 = value << 1; // 12 * 2^1 = 24
            int leftShift2 = value << 2; // 12 * 2^2 = 48
            int leftShift3 = value << 3; // 12 * 2^3 = 96
            
            Console.WriteLine($"   Left shift by 1: {leftShift1} (Binary: {Convert.ToString(leftShift1, 2).PadLeft(8, '0')})");
            Console.WriteLine($"   Left shift by 2: {leftShift2} (Binary: {Convert.ToString(leftShift2, 2).PadLeft(8, '0')})");
            Console.WriteLine($"   Left shift by 3: {leftShift3} (Binary: {Convert.ToString(leftShift3, 2).PadLeft(8, '0')})");
            
            // Right shift - divides by 2^n (integer division)
            int rightShift1 = value >> 1; // 12 / 2^1 = 6
            int rightShift2 = value >> 2; // 12 / 2^2 = 3
            int rightShift3 = value >> 3; // 12 / 2^3 = 1
            
            Console.WriteLine($"   Right shift by 1: {rightShift1} (Binary: {Convert.ToString(rightShift1, 2).PadLeft(8, '0')})");
            Console.WriteLine($"   Right shift by 2: {rightShift2} (Binary: {Convert.ToString(rightShift2, 2).PadLeft(8, '0')})");
            Console.WriteLine($"   Right shift by 3: {rightShift3} (Binary: {Convert.ToString(rightShift3, 2).PadLeft(8, '0')})");
            Console.WriteLine();
        }

        /// <summary>
        /// Shows difference between arithmetic and logical right shift
        /// </summary>
        static void ArithmeticVsLogicalShift()
        {
            Console.WriteLine("2. Arithmetic vs Logical Right Shift:");
            
            // Signed types use arithmetic right shift (preserves sign)
            int signedNegative = -8; // Binary: 11111111111111111111111111111000
            int signedShifted = signedNegative >> 2;
            Console.WriteLine($"   Signed -8 >> 2 = {signedShifted} (Arithmetic shift - preserves sign)");
            
            // Unsigned types use logical right shift (fills with zeros)
            uint unsignedValue = 0xFFFFFFFC; // Same bit pattern as -4
            uint unsignedShifted = unsignedValue >> 2;
            Console.WriteLine($"   Unsigned 0xFFFFFFFC >> 2 = {unsignedShifted} (Logical shift - fills with zeros)");
            
            // Demonstrating sign extension
            sbyte signedByte = -16; // Binary: 11110000
            int promoted = signedByte >> 2;
            Console.WriteLine($"   sbyte -16 >> 2 = {promoted} (Sign extended: {Convert.ToString(promoted, 2)})");
            Console.WriteLine();
        }

        /// <summary>
        /// Performance optimization using shifts for multiplication/division
        /// MEMORY ALLOCATION: No heap allocation, pure CPU operations
        /// </summary>
        static void MultiplicationDivisionOptimization()
        {
            Console.WriteLine("3. Multiplication/Division Optimization:");
            
            int number = 100;
            
            // Fast multiplication by powers of 2
            Console.WriteLine($"   Original: {number}");
            Console.WriteLine($"   Multiply by 2:  {number << 1} (equivalent to {number} * 2)");
            Console.WriteLine($"   Multiply by 4:  {number << 2} (equivalent to {number} * 4)");
            Console.WriteLine($"   Multiply by 8:  {number << 3} (equivalent to {number} * 8)");
            Console.WriteLine($"   Multiply by 16: {number << 4} (equivalent to {number} * 16)");
            
            // Fast division by powers of 2
            Console.WriteLine($"   Divide by 2:  {number >> 1} (equivalent to {number} / 2)");
            Console.WriteLine($"   Divide by 4:  {number >> 2} (equivalent to {number} / 4)");
            Console.WriteLine($"   Divide by 8:  {number >> 3} (equivalent to {number} / 8)");
            Console.WriteLine($"   Divide by 16: {number >> 4} (equivalent to {number} / 16)");
            
            // Complex calculations using shifts
            int area = CalculateAreaOptimized(10, 8);
            Console.WriteLine($"   Optimized area calculation (10 * 8): {area}");
            Console.WriteLine();
        }

        /// <summary>
        /// Optimized area calculation using bit shifts
        /// </summary>
        static int CalculateAreaOptimized(int width, int height)
        {
            // If one dimension is a power of 2, use shifting
            if (IsPowerOfTwo(width))
            {
                return height << (int)Math.Log2(width);
            }
            else if (IsPowerOfTwo(height))
            {
                return width << (int)Math.Log2(height);
            }
            else
            {
                return width * height; // Fall back to normal multiplication
            }
        }

        /// <summary>
        /// Bit manipulation techniques using shift operators
        /// </summary>
        static void BitManipulationTechniques()
        {
            Console.WriteLine("4. Bit Manipulation Techniques:");
            
            uint value = 0x12345678;
            Console.WriteLine($"   Original value: 0x{value:X8}");
            
            // Extract bytes using shifts
            byte byte0 = (byte)(value & 0xFF);           // LSB
            byte byte1 = (byte)((value >> 8) & 0xFF);
            byte byte2 = (byte)((value >> 16) & 0xFF);
            byte byte3 = (byte)((value >> 24) & 0xFF);   // MSB
            
            Console.WriteLine($"   Byte 0 (LSB): 0x{byte0:X2}");
            Console.WriteLine($"   Byte 1:       0x{byte1:X2}");
            Console.WriteLine($"   Byte 2:       0x{byte2:X2}");
            Console.WriteLine($"   Byte 3 (MSB): 0x{byte3:X2}");
            
            // Reconstruct value
            uint reconstructed = ((uint)byte3 << 24) | ((uint)byte2 << 16) | ((uint)byte1 << 8) | byte0;
            Console.WriteLine($"   Reconstructed: 0x{reconstructed:X8}");
            
            // Bit field extraction
            uint field = ExtractBitField(value, 8, 16); // Extract 16 bits starting at position 8
            Console.WriteLine($"   Extracted field (bits 8-23): 0x{field:X4}");
            Console.WriteLine();
        }

        /// <summary>
        /// Extract a bit field from a value
        /// </summary>
        static uint ExtractBitField(uint value, int startBit, int length)
        {
            uint mask = (1u << length) - 1; // Create mask with 'length' ones
            return (value >> startBit) & mask;
        }

        /// <summary>
        /// Flag operations using shift operators
        /// </summary>
        static void FlagOperations()
        {
            Console.WriteLine("5. Flag Operations:");
            
            // Define flag positions using shifts
            const int FLAG_READ = 1 << 0;     // 0001
            const int FLAG_WRITE = 1 << 1;    // 0010
            const int FLAG_EXECUTE = 1 << 2;  // 0100
            const int FLAG_ADMIN = 1 << 3;    // 1000
            
            Console.WriteLine($"   FLAG_READ:    {FLAG_READ:D2} (Binary: {Convert.ToString(FLAG_READ, 2).PadLeft(4, '0')})");
            Console.WriteLine($"   FLAG_WRITE:   {FLAG_WRITE:D2} (Binary: {Convert.ToString(FLAG_WRITE, 2).PadLeft(4, '0')})");
            Console.WriteLine($"   FLAG_EXECUTE: {FLAG_EXECUTE:D2} (Binary: {Convert.ToString(FLAG_EXECUTE, 2).PadLeft(4, '0')})");
            Console.WriteLine($"   FLAG_ADMIN:   {FLAG_ADMIN:D2} (Binary: {Convert.ToString(FLAG_ADMIN, 2).PadLeft(4, '0')})");
            
            // Combine flags
            int permissions = FLAG_READ | FLAG_WRITE;
            Console.WriteLine($"   Combined permissions: {permissions} (Binary: {Convert.ToString(permissions, 2).PadLeft(4, '0')})");
            
            // Check flags
            bool hasRead = (permissions & FLAG_READ) != 0;
            bool hasExecute = (permissions & FLAG_EXECUTE) != 0;
            Console.WriteLine($"   Has READ permission: {hasRead}");
            Console.WriteLine($"   Has EXECUTE permission: {hasExecute}");
            Console.WriteLine();
        }

        /// <summary>
        /// Hash function implementation using shift operators
        /// </summary>
        static void HashingExample()
        {
            Console.WriteLine("6. Hashing Example:");
            
            string[] testStrings = { "Hello", "World", "C#", "Shift", "Operators" };
            
            foreach (string str in testStrings)
            {
                uint hash = SimpleHash(str);
                Console.WriteLine($"   Hash of '{str}': 0x{hash:X8}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Simple hash function using shift and XOR operations
        /// </summary>
        static uint SimpleHash(string input)
        {
            uint hash = 5381; // Magic number used in djb2 hash
            
            foreach (char c in input)
            {
                hash = ((hash << 5) + hash) + c; // hash * 33 + c
            }
            
            return hash;
        }

        /// <summary>
        /// Color manipulation using bit shifts for graphics programming
        /// </summary>
        static void ColorManipulation()
        {
            Console.WriteLine("7. Color Manipulation (ARGB):");
            
            // ARGB color: Alpha=255, Red=128, Green=64, Blue=32
            uint color = 0xFF804020;
            Console.WriteLine($"   Original color: 0x{color:X8}");
            
            // Extract color components using shifts
            byte alpha = (byte)((color >> 24) & 0xFF);
            byte red = (byte)((color >> 16) & 0xFF);
            byte green = (byte)((color >> 8) & 0xFF);
            byte blue = (byte)(color & 0xFF);
            
            Console.WriteLine($"   Alpha: {alpha}, Red: {red}, Green: {green}, Blue: {blue}");
            
            // Create new color with modified components
            byte newRed = (byte)(red >> 1);    // Halve red intensity
            byte newGreen = (byte)(green << 1); // Double green intensity (with overflow check)
            if (newGreen < green) newGreen = 255; // Handle overflow
            
            uint newColor = ((uint)alpha << 24) | ((uint)newRed << 16) | ((uint)newGreen << 8) | blue;
            Console.WriteLine($"   Modified color: 0x{newColor:X8}");
            Console.WriteLine($"   New components - Red: {newRed}, Green: {newGreen}");
            Console.WriteLine();
        }

        /// <summary>
        /// Performance comparison between shift and traditional arithmetic
        /// MEMORY ALLOCATION: All operations are stack-based, no heap allocation
        /// </summary>
        static void PerformanceComparison()
        {
            Console.WriteLine("8. Performance Comparison:");
            
            const int iterations = 10_000_000;
            int value = 123456;
            
            // Test multiplication performance
            Stopwatch sw = Stopwatch.StartNew();
            long traditionalSum = 0;
            for (int i = 0; i < iterations; i++)
            {
                traditionalSum += value * 8;
            }
            sw.Stop();
            long traditionalTime = sw.ElapsedMilliseconds;
            
            // Test shift performance
            sw.Restart();
            long shiftSum = 0;
            for (int i = 0; i < iterations; i++)
            {
                shiftSum += value << 3; // Multiply by 8
            }
            sw.Stop();
            long shiftTime = sw.ElapsedMilliseconds;
            
            Console.WriteLine($"   Traditional multiplication (* 8): {traditionalTime}ms");
            Console.WriteLine($"   Shift operation (<< 3):          {shiftTime}ms");
            Console.WriteLine($"   Performance improvement:          {(double)traditionalTime / shiftTime:F2}x faster");
            Console.WriteLine($"   Results match: {traditionalSum == shiftSum}");
            Console.WriteLine();
        }

        /// <summary>
        /// Multithreading example with shift operations
        /// MULTITHREADING: Demonstrates thread-safe usage patterns
        /// </summary>
        static void MultithreadingExample()
        {
            Console.WriteLine("9. Multithreading Example:");
            
            const int arraySize = 1000000;
            int[] sourceArray = new int[arraySize];
            int[] resultArray = new int[arraySize];
            
            // Initialize source array
            for (int i = 0; i < arraySize; i++)
            {
                sourceArray[i] = i + 1;
            }
            
            // Parallel processing using shift operations
            Stopwatch sw = Stopwatch.StartNew();
            Parallel.For(0, arraySize, i =>
            {
                // Each thread independently processes its assigned elements
                // Shift operations are inherently thread-safe for local operations
                resultArray[i] = (sourceArray[i] << 2) + (sourceArray[i] >> 1); // *4 + /2
            });
            sw.Stop();
            
            Console.WriteLine($"   Processed {arraySize} elements in {sw.ElapsedMilliseconds}ms");
            Console.WriteLine($"   Sample results: {resultArray[0]}, {resultArray[100]}, {resultArray[1000]}");
            
            // Thread-safe counter using atomic operations and shifts
            int counter = 0;
            const int incrementCount = 10000;
            
            Parallel.For(0, incrementCount, i =>
            {
                // Use Interlocked for thread-safe operations on shared variables
                int increment = 1 << (i % 4); // Varying increment: 1, 2, 4, 8
                Interlocked.Add(ref counter, increment);
            });
            
            Console.WriteLine($"   Thread-safe counter result: {counter}");
            Console.WriteLine();
        }

        /// <summary>
        /// Edge cases and common pitfalls with shift operators
        /// </summary>
        static void EdgeCasesAndPitfalls()
        {
            Console.WriteLine("10. Edge Cases and Pitfalls:");
            
            // Shift count larger than type size
            int value = 1;
            int largeShift = value << 32; // On 32-bit int, this is undefined behavior
            Console.WriteLine($"    Shift by 32 bits (undefined): {largeShift}");
            
            // Negative shift counts (compile-time error)
            // int negativeShift = value << -1; // This won't compile
            
            // Overflow in left shift
            int maxInt = int.MaxValue;
            int overflowed = maxInt << 1; // This will overflow
            Console.WriteLine($"    MaxInt << 1 (overflow): {overflowed}");
            
            // Right shift of negative numbers
            int negative = -10;
            int rightShiftNegative = negative >> 2;
            Console.WriteLine($"    -10 >> 2 (arithmetic): {rightShiftNegative}");
            
            // Precision loss in right shift
            int oddNumber = 15;
            int divided = oddNumber >> 1; // 15 / 2 = 7.5, but result is 7
            Console.WriteLine($"    15 >> 1 (precision loss): {divided} (not 7.5)");
            
            // Shift count masking (only lower 5 bits used for int)
            int masked = value << 33; // Equivalent to value << 1
            Console.WriteLine($"    1 << 33 (masked to 1): {masked}");
            Console.WriteLine();
        }

        /// <summary>
        /// Real-world applications of shift operators
        /// </summary>
        static void RealWorldApplications()
        {
            Console.WriteLine("11. Real-World Applications:");
            
            // Binary tree node indexing
            Console.WriteLine("    Binary Tree Navigation:");
            int nodeIndex = 5;
            int parentIndex = nodeIndex >> 1;        // Parent at index/2
            int leftChild = nodeIndex << 1;          // Left child at index*2
            int rightChild = (nodeIndex << 1) + 1;   // Right child at index*2+1
            Console.WriteLine($"      Node {nodeIndex}: Parent={parentIndex}, Left={leftChild}, Right={rightChild}");
            
            // Memory alignment
            Console.WriteLine("    Memory Alignment:");
            IntPtr address = new IntPtr(0x12345678);
            IntPtr aligned16 = new IntPtr((address.ToInt64() + 15) & ~15); // Align to 16-byte boundary
            Console.WriteLine($"      Original: 0x{address.ToInt64():X}, Aligned: 0x{aligned16.ToInt64():X}");
            
            // Fast modulo for power-of-2 divisors
            Console.WriteLine("    Fast Modulo (Power of 2):");
            int number = 1000;
            int mod8Traditional = number % 8;
            int mod8Fast = number & 7;  // Equivalent to number & (8-1)
            Console.WriteLine($"      {number} % 8 = {mod8Traditional} (traditional), {mod8Fast} (bitwise)");
            
            // Checksum calculation
            Console.WriteLine("    Simple Checksum:");
            byte[] data = { 0x12, 0x34, 0x56, 0x78 };
            uint checksum = CalculateSimpleChecksum(data);
            Console.WriteLine($"      Checksum of data: 0x{checksum:X8}");
            
            Console.WriteLine();
        }

        /// <summary>
        /// Calculate a simple checksum using shift operations
        /// </summary>
        static uint CalculateSimpleChecksum(byte[] data)
        {
            uint checksum = 0;
            for (int i = 0; i < data.Length; i++)
            {
                checksum = (checksum << 1) ^ data[i]; // Rotate left and XOR
            }
            return checksum;
        }

        /// <summary>
        /// Check if a number is a power of 2
        /// </summary>
        static bool IsPowerOfTwo(int number)
        {
            return number > 0 && (number & (number - 1)) == 0;
        }
    }
}
