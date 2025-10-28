using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

/*
 * FLYWEIGHT DESIGN PATTERN EXAMPLE
 * 
 * PURPOSE:
 * The Flyweight pattern is used to minimize memory usage when dealing with large numbers of objects
 * by sharing common data between multiple objects. It separates intrinsic state (shared) from 
 * extrinsic state (context-dependent).
 * 
 * MEMORY ALLOCATION BENEFITS:
 * - Reduces memory footprint by sharing common object instances
 * - Prevents object duplication for similar data
 * - Particularly effective when you have thousands or millions of similar objects
 * - Intrinsic state is stored once and shared; extrinsic state is passed as parameters
 * 
 * SCENARIOS TO USE:
 * - Large numbers of similar objects (game sprites, text characters, UI components)
 * - Objects with significant shared state
 * - Memory-constrained environments
 * - When object creation cost is high
 * - Text editors (character formatting), games (particles, bullets), graphics systems
 * 
 * SCENARIOS NOT TO USE:
 * - Small number of objects where memory isn't a concern
 * - Objects with mostly unique state (little to share)
 * - When the complexity of separating intrinsic/extrinsic state outweighs benefits
 * - Real-time systems where the lookup overhead is critical
 * - When objects need to be modified frequently (flyweights should be immutable)
 * 
 * MULTITHREADING ASPECTS:
 * - Flyweight objects should be immutable to ensure thread safety
 * - Factory must be thread-safe (use ConcurrentDictionary or locks)
 * - Extrinsic state should be passed per operation, not stored in flyweight
 * - Shared intrinsic state eliminates race conditions if properly designed
 */

namespace FlyweightPattern
{
    // Flyweight interface defining operations that can act on both intrinsic and extrinsic state
    public interface ICharacterFlyweight
    {
        // Operation method that takes extrinsic state (position, size) as parameters
        // Intrinsic state (font, style) is stored in the flyweight itself
        void Display(int x, int y, int size, ConsoleColor color);
    }

    // Concrete Flyweight: stores intrinsic state and implements flyweight interface
    // THREAD SAFETY: This class is immutable, making it inherently thread-safe
    public class CharacterFlyweight : ICharacterFlyweight
    {
        // Intrinsic state - shared among multiple contexts
        // These values don't change and are shared across all instances with same character/font
        private readonly char _character;
        private readonly string _fontFamily;
        private readonly bool _isBold;

        // Constructor accepts only intrinsic state
        public CharacterFlyweight(char character, string fontFamily, bool isBold)
        {
            _character = character;
            _fontFamily = fontFamily;
            _isBold = isBold;
            
            // Simulate memory allocation cost
            Console.WriteLine($"[MEMORY] Creating flyweight for '{character}' with {fontFamily} font");
        }

        // Display method accepts extrinsic state as parameters
        // MEMORY EFFICIENCY: Position, size, and color are not stored in the object
        public void Display(int x, int y, int size, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.SetCursorPosition(x, y);
            
            // Use both intrinsic state (character, font) and extrinsic state (position, color)
            string output = _isBold ? $"**{_character}**" : _character.ToString();
            Console.Write($"{output}({_fontFamily},s{size})");
            
            Console.ResetColor();
        }

        // Read-only properties to access intrinsic state
        public char Character => _character;
        public string FontFamily => _fontFamily;
        public bool IsBold => _isBold;
    }

    // Flyweight Factory: manages flyweight instances and ensures sharing
    // THREAD SAFETY: Uses ConcurrentDictionary for thread-safe operations
    public class CharacterFlyweightFactory
    {
        // Thread-safe collection to store shared flyweight instances
        // MEMORY OPTIMIZATION: Prevents duplicate flyweights for same intrinsic state
        private static readonly ConcurrentDictionary<string, ICharacterFlyweight> _flyweights = 
            new ConcurrentDictionary<string, ICharacterFlyweight>();

        // Factory method to get or create flyweight instances
        // THREAD SAFETY: ConcurrentDictionary.GetOrAdd is atomic and thread-safe
        public static ICharacterFlyweight GetCharacterFlyweight(char character, string fontFamily, bool isBold)
        {
            // Create unique key for intrinsic state combination
            string key = $"{character}_{fontFamily}_{isBold}";
            
            // GetOrAdd ensures thread-safe creation and retrieval
            // If key exists, returns existing flyweight; otherwise creates new one
            return _flyweights.GetOrAdd(key, k => 
            {
                Console.WriteLine($"[FACTORY] Creating new flyweight for key: {key}");
                return new CharacterFlyweight(character, fontFamily, isBold);
            });
        }

        // Method to get statistics about flyweight usage
        // MEMORY MONITORING: Helps track memory efficiency
        public static void PrintFlyweightStats()
        {
            Console.WriteLine($"\n[STATS] Total flyweight instances created: {_flyweights.Count}");
            Console.WriteLine("[STATS] Flyweight instances:");
            foreach (var kvp in _flyweights)
            {
                Console.WriteLine($"  - Key: {kvp.Key}");
            }
        }

        // Method to clear flyweights (useful for testing or memory cleanup)
        public static void ClearFlyweights()
        {
            _flyweights.Clear();
            Console.WriteLine("[FACTORY] All flyweights cleared");
        }
    }

    // Context class that uses flyweights
    // EXTRINSIC STATE: Stores position, size, and color information
    public class Character
    {
        // Extrinsic state - unique per character instance
        private int _x, _y, _size;
        private ConsoleColor _color;
        
        // Reference to flyweight (intrinsic state)
        private readonly ICharacterFlyweight _flyweight;

        public Character(char character, string fontFamily, bool isBold, 
                        int x, int y, int size, ConsoleColor color)
        {
            // Get shared flyweight instance from factory
            _flyweight = CharacterFlyweightFactory.GetCharacterFlyweight(character, fontFamily, isBold);
            
            // Store extrinsic state
            _x = x;
            _y = y;
            _size = size;
            _color = color;
        }

        // Method to display character using both intrinsic and extrinsic state
        public void Display()
        {
            _flyweight.Display(_x, _y, _size, _color);
        }

        // Methods to modify extrinsic state
        public void Move(int newX, int newY)
        {
            _x = newX;
            _y = newY;
        }

        public void ChangeColor(ConsoleColor newColor)
        {
            _color = newColor;
        }
    }

    // Document class to demonstrate flyweight usage with multiple characters
    public class Document
    {
        private readonly List<Character> _characters = new List<Character>();

        public void AddCharacter(char ch, string font, bool bold, int x, int y, int size, ConsoleColor color)
        {
            _characters.Add(new Character(ch, font, bold, x, y, size, color));
        }

        public void Display()
        {
            Console.Clear();
            Console.WriteLine("Document Display (flyweight pattern demonstration):");
            Console.WriteLine(new string('-', 60));
            
            foreach (var character in _characters)
            {
                character.Display();
            }
            
            Console.SetCursorPosition(0, Console.WindowHeight - 3);
        }

        public int CharacterCount => _characters.Count;
    }

    // Demonstration of multithreading with flyweights
    public class MultithreadingDemo
    {
        public static async Task RunConcurrentTest()
        {
            Console.WriteLine("\n[MULTITHREAD TEST] Creating characters concurrently...");
            
            // Create multiple tasks that create characters concurrently
            var tasks = new Task[10];
            
            for (int i = 0; i < tasks.Length; i++)
            {
                int taskId = i;
                tasks[i] = Task.Run(() =>
                {
                    // Each task creates characters with same intrinsic state
                    // This tests thread safety of the flyweight factory
                    for (int j = 0; j < 5; j++)
                    {
                        var character = new Character('A', "Arial", true, 
                                                    taskId * 2, j + 5, 12, ConsoleColor.Green);
                        
                        Console.WriteLine($"[THREAD {taskId}] Created character at ({taskId * 2}, {j + 5})");
                        
                        // Small delay to increase chance of concurrent access
                        Thread.Sleep(10);
                    }
                });
            }
            
            await Task.WhenAll(tasks);
            Console.WriteLine("[MULTITHREAD TEST] All tasks completed successfully");
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== FLYWEIGHT PATTERN DEMONSTRATION ===\n");

            // Example 1: Basic flyweight usage
            Console.WriteLine("1. BASIC FLYWEIGHT USAGE:");
            Console.WriteLine("Creating document with repeated characters...\n");

            var document = new Document();

            // Add many characters - notice that flyweights are reused for same char/font combinations
            // MEMORY EFFICIENCY: Even though we create 20 Character objects, 
            // only a few flyweight instances are actually created
            document.AddCharacter('H', "Arial", true, 2, 2, 14, ConsoleColor.Red);
            document.AddCharacter('e', "Arial", false, 4, 2, 14, ConsoleColor.Blue);
            document.AddCharacter('l', "Arial", false, 6, 2, 14, ConsoleColor.Green);
            document.AddCharacter('l', "Arial", false, 8, 2, 14, ConsoleColor.Green); // Reuses 'l' flyweight
            document.AddCharacter('o', "Arial", false, 10, 2, 14, ConsoleColor.Yellow);
            
            // Add more characters with different extrinsic state but same intrinsic state
            document.AddCharacter('H', "Arial", true, 2, 3, 16, ConsoleColor.Magenta); // Reuses 'H' flyweight
            document.AddCharacter('e', "Arial", false, 4, 3, 16, ConsoleColor.Cyan);   // Reuses 'e' flyweight

            document.Display();
            
            Console.WriteLine($"\nTotal characters in document: {document.CharacterCount}");
            CharacterFlyweightFactory.PrintFlyweightStats();

            Console.WriteLine("\nPress any key to continue to multithreading demo...");
            Console.ReadKey();

            // Example 2: Multithreading demonstration
            Console.WriteLine("\n\n2. MULTITHREADING DEMONSTRATION:");
            await MultithreadingDemo.RunConcurrentTest();
            
            CharacterFlyweightFactory.PrintFlyweightStats();

            // Example 3: Memory efficiency comparison
            Console.WriteLine("\n\n3. MEMORY EFFICIENCY COMPARISON:");
            Console.WriteLine("Without flyweight: Each character object would store font, style, AND position");
            Console.WriteLine("With flyweight: Font and style are shared; only position is stored per character");
            Console.WriteLine($"Characters created: {document.CharacterCount}");
            Console.WriteLine("Flyweight instances: See stats above");
            Console.WriteLine("\nMemory saved = (Total characters - Unique flyweights) * Size of intrinsic state");

            // Example 4: Demonstrate immutability for thread safety
            Console.WriteLine("\n\n4. FLYWEIGHT IMMUTABILITY (Thread Safety):");
            var flyweight1 = CharacterFlyweightFactory.GetCharacterFlyweight('X', "Times", true);
            var flyweight2 = CharacterFlyweightFactory.GetCharacterFlyweight('X', "Times", true);
            
            Console.WriteLine($"Same flyweight instance returned: {ReferenceEquals(flyweight1, flyweight2)}");
            Console.WriteLine("Flyweights are immutable - thread-safe by design");

            Console.WriteLine("\n\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
