using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;

/*
 * THE "IS" KEYWORD IN C#
 * 
 * PURPOSE:
 * The 'is' keyword in C# is used for type checking and pattern matching.
 * It allows you to:
 * - Check if an object is of a specific type (type testing)
 * - Perform pattern matching with immediate variable assignment
 * - Test for null values safely
 * - Match constant values, properties, and complex patterns
 * 
 * CORE BENEFITS:
 * - Type-safe casting without throwing exceptions
 * - Combines type checking and casting in a single operation
 * - Supports modern pattern matching syntax (C# 7.0+)
 * - Null-safe operations
 * - More readable and concise than traditional casting
 * 
 * SCENARIOS TO USE:
 * - When you need to check and cast objects safely
 * - Polymorphic scenarios where you need to handle different derived types
 * - When working with object hierarchies or interfaces
 * - Pattern matching for value extraction
 * - Null checking and safe navigation
 * - Switch expressions with pattern matching
 * - When you want to avoid InvalidCastException
 * 
 * SCENARIOS NOT TO USE:
 * - When you're certain of the type (use direct casting)
 * - Performance-critical loops where type checking overhead matters
 * - When 'as' operator with null checking is more appropriate
 * - Simple equality comparisons (use == instead)
 * - When reflection might be more suitable for complex type operations
 */

namespace IsKeywordExamples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            #error version
            
            Console.WriteLine("=== C# 'is' Keyword Examples ===\n");

            // Example 1: Basic Type Testing
            BasicTypeTestingExample();

            // Example 2: Pattern Matching with Variable Assignment
            PatternMatchingExample();

            // Example 3: Polymorphism and Inheritance
            PolymorphismExample();

            // Example 4: Null Checking and Value Testing
            NullCheckingExample();

            // Example 5: Advanced Pattern Matching (C# 8.0+)
            AdvancedPatternMatchingExample();

            // Example 6: Performance Considerations
            PerformanceExample();

            // Example 7: Thread Safety Considerations
            await ThreadSafetyExample();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        #region Example 1: Basic Type Testing

        static void BasicTypeTestingExample()
        {
            Console.WriteLine("1. Basic Type Testing:");
            Console.WriteLine("======================");

            object[] items = { 42, "Hello", 3.14, true, new DateTime(2025, 1, 1) };

            foreach (var item in items)
            {
                Console.WriteLine($"Item: {item}");

                // Traditional type checking
                if (item.GetType() == typeof(int))
                {
                    Console.WriteLine("  Traditional check: This is an integer");
                }

                // Using 'is' keyword for type testing
                // MEMORY ALLOCATION: No additional allocation, just type comparison
                if (item is int)
                {
                    Console.WriteLine("  'is' check: This is an integer");
                    // Note: item is still of type object here, need casting to use as int
                    int value = (int)item;
                    Console.WriteLine($"    Integer value: {value}");
                }

                // 'is' with pattern matching (C# 7.0+)
                // MEMORY ALLOCATION: No additional allocation, immediate variable assignment
                if (item is int intValue)
                {
                    Console.WriteLine($"  'is' with pattern: Integer value is {intValue}");
                    // intValue is available as int type immediately
                }

                if (item is string stringValue)
                {
                    Console.WriteLine($"  'is' with pattern: String value is '{stringValue}'");
                    Console.WriteLine($"    String length: {stringValue.Length}");
                }

                if (item is double doubleValue)
                {
                    Console.WriteLine($"  'is' with pattern: Double value is {doubleValue:F2}");
                }

                Console.WriteLine();
            }
        }

        #endregion

        #region Example 2: Pattern Matching with Variable Assignment

        static void PatternMatchingExample()
        {
            Console.WriteLine("2. Pattern Matching with Variable Assignment:");
            Console.WriteLine("=============================================");

            var shapes = new List<Shape?>
            {
                new Circle(5.0),
                new Rectangle(4.0, 6.0),
                new Triangle(3.0, 4.0, 5.0),
                null
            };

            foreach (var shape in shapes)
            {
                // Pattern matching with 'is' keyword
                // Each branch creates a new variable with the correct type
                if (shape is Circle circle)
                {
                    Console.WriteLine($"Circle with radius {circle.Radius}, Area: {circle.CalculateArea():F2}");
                }
                else if (shape is Rectangle rectangle)
                {
                    Console.WriteLine($"Rectangle {rectangle.Width}x{rectangle.Height}, Area: {rectangle.CalculateArea():F2}");
                }
                else if (shape is Triangle triangle)
                {
                    Console.WriteLine($"Triangle with sides ({triangle.SideA}, {triangle.SideB}, {triangle.SideC}), Area: {triangle.CalculateArea():F2}");
                }
                else if (shape is null)
                {
                    Console.WriteLine("Null shape encountered");
                }
                else
                {
                    Console.WriteLine($"Unknown shape type: {shape.GetType().Name}");
                }
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 3: Polymorphism and Inheritance

        static void PolymorphismExample()
        {
            Console.WriteLine("3. Polymorphism and Inheritance:");
            Console.WriteLine("================================");

            var animals = new List<Animal>
            {
                new Dog("Buddy", "Golden Retriever"),
                new Cat("Whiskers", 9),
                new Bird("Tweety", true),
                new Fish("Nemo", "Saltwater")
            };

            foreach (var animal in animals)
            {
                Console.WriteLine($"Animal: {animal.Name}");
                animal.MakeSound();

                // Use 'is' to access specific derived class members
                // MEMORY ALLOCATION: No boxing/unboxing, just reference comparison
                switch (animal)
                {
                    case Dog dog when dog.Breed.Contains("Retriever"):
                        Console.WriteLine($"  This is a retriever dog. Breed: {dog.Breed}");
                        dog.Fetch();
                        break;

                    case Dog dog:
                        Console.WriteLine($"  Regular dog. Breed: {dog.Breed}");
                        dog.Fetch();
                        break;

                    case Cat cat when cat.Lives > 5:
                        Console.WriteLine($"  Cat with many lives remaining: {cat.Lives}");
                        cat.Purr();
                        break;

                    case Cat cat:
                        Console.WriteLine($"  Cat with few lives: {cat.Lives}");
                        cat.Purr();
                        break;

                    case Bird bird:
                        Console.WriteLine($"  Bird that {(bird.CanFly ? "can" : "cannot")} fly");
                        bird.Fly();
                        break;

                    case Fish fish:
                        Console.WriteLine($"  Fish in {fish.WaterType} environment");
                        fish.Swim();
                        break;
                }

                Console.WriteLine();
            }
        }

        #endregion

        #region Example 4: Null Checking and Value Testing

        static void NullCheckingExample()
        {
            Console.WriteLine("4. Null Checking and Value Testing:");
            Console.WriteLine("====================================");

            string? nullableString = null;
            string? anotherString = "Hello World";
            int? nullableInt = null;
            int? anotherInt = 42;

            // Null checking with 'is'
            if (nullableString is null)
            {
                Console.WriteLine("nullableString is null");
            }

            if (anotherString is not null)
            {
                Console.WriteLine($"anotherString is not null: '{anotherString}'");
            }

            // Pattern matching with null checking
            if (nullableInt is int value)
            {
                Console.WriteLine($"nullableInt has value: {value}");
            }
            else
            {
                Console.WriteLine("nullableInt is null");
            }

            if (anotherInt is int anotherValue)
            {
                Console.WriteLine($"anotherInt has value: {anotherValue}");
            }

            // Constant pattern matching
            var testValues = new[] { 0, 1, 42, 100, -1 };
            
            foreach (var testValue in testValues)
            {
                var description = testValue switch
                {
                    0 => "Zero",
                    1 => "One", 
                    42 => "The Answer",
                    var n when n > 0 => "Positive number",
                    var n when n < 0 => "Negative number",
                    _ => "Unknown"
                };
                
                Console.WriteLine($"Value {testValue} is: {description}");
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 5: Advanced Pattern Matching (C# 8.0+)

        static void AdvancedPatternMatchingExample()
        {
            Console.WriteLine("5. Advanced Pattern Matching (C# 8.0+):");
            Console.WriteLine("=========================================");

            var people = new List<Person>
            {
                new Person("Alice", 25, "Engineer"),
                new Person("Bob", 30, "Manager"),
                new Person("Charlie", 35, "Developer"),
                new Person("Diana", 28, "Designer"),
                new Person("Eve", 22, "Intern")
            };

            foreach (var person in people)
            {
                // Property pattern matching
                var category = person switch
                {
                    { Age: < 25 } => "Young Professional",
                    { Age: >= 25 and < 30, Job: "Engineer" } => "Mid-level Engineer",
                    { Age: >= 30, Job: "Manager" } => "Senior Manager",
                    { Job: "Developer" } => "Software Developer",
                    { Age: var age } when age > 35 => "Senior Professional",
                    _ => "Professional"
                };

                Console.WriteLine($"{person.Name} ({person.Age}, {person.Job}) -> {category}");

                // Tuple pattern matching
                var workSchedule = (person.Job, person.Age) switch
                {
                    ("Intern", _) => "Flexible hours",
                    ("Manager", > 30) => "Executive schedule",
                    ("Engineer" or "Developer", >= 25) => "Standard development hours",
                    ("Designer", _) => "Creative hours",
                    _ => "Regular business hours"
                };

                Console.WriteLine($"  Work schedule: {workSchedule}");
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 6: Performance Considerations

        static void PerformanceExample()
        {
            Console.WriteLine("6. Performance Considerations:");
            Console.WriteLine("===============================");

            const int iterations = 1_000_000;
            var random = new Random();
            var testObjects = new object[100];
            
            // Fill with random types
            for (int i = 0; i < testObjects.Length; i++)
            {
                testObjects[i] = random.Next(4) switch
                {
                    0 => random.Next(1000),
                    1 => $"String{random.Next(100)}",
                    2 => random.NextDouble(),
                    _ => DateTime.Now.AddDays(random.Next(-365, 365))
                };
            }

            // Benchmark different approaches
            Console.WriteLine("Performance comparison (1M iterations):");

            // Method 1: Traditional casting with try-catch
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            int count1 = 0;
            
            for (int i = 0; i < iterations; i++)
            {
                var obj = testObjects[i % testObjects.Length];
                try
                {
                    var intValue = (int)obj;
                    count1++;
                }
                catch (InvalidCastException)
                {
                    // Not an int
                }
            }
            
            stopwatch.Stop();
            Console.WriteLine($"  Try-catch casting: {stopwatch.ElapsedMilliseconds}ms, Count: {count1}");

            // Method 2: Using 'is' operator
            stopwatch.Restart();
            int count2 = 0;
            
            for (int i = 0; i < iterations; i++)
            {
                var obj = testObjects[i % testObjects.Length];
                if (obj is int)
                {
                    var intValue = (int)obj;
                    count2++;
                }
            }
            
            stopwatch.Stop();
            Console.WriteLine($"  'is' + cast: {stopwatch.ElapsedMilliseconds}ms, Count: {count2}");

            // Method 3: Using 'is' with pattern matching
            stopwatch.Restart();
            int count3 = 0;
            
            for (int i = 0; i < iterations; i++)
            {
                var obj = testObjects[i % testObjects.Length];
                if (obj is int intValue)
                {
                    // intValue is already the correct type
                    count3++;
                }
            }
            
            stopwatch.Stop();
            Console.WriteLine($"  'is' with pattern: {stopwatch.ElapsedMilliseconds}ms, Count: {count3}");

            // MEMORY ALLOCATION: 'is' pattern matching is generally most efficient
            // as it combines type checking and assignment without additional allocations
            Console.WriteLine("\nMemory allocation notes:");
            Console.WriteLine("  - 'is' pattern matching: Most efficient, no extra allocations");
            Console.WriteLine("  - 'is' + cast: Slightly less efficient due to double type check");
            Console.WriteLine("  - Try-catch: Worst performance due to exception handling overhead");

            Console.WriteLine();
        }

        #endregion

        #region Example 7: Thread Safety Considerations

        static async Task ThreadSafetyExample()
        {
            Console.WriteLine("7. Thread Safety Considerations:");
            Console.WriteLine("=================================");

            var sharedQueue = new ConcurrentQueue<object>();
            var results = new ConcurrentDictionary<string, int>();
            
            // Add various objects to the queue
            var random = new Random();
            for (int i = 0; i < 1000; i++)
            {
                object obj = random.Next(3) switch
                {
                    0 => random.Next(100),
                    1 => $"String{random.Next(100)}",
                    _ => random.NextDouble()
                };
                sharedQueue.Enqueue(obj);
            }

            Console.WriteLine($"Added {sharedQueue.Count} objects to concurrent queue");

            // Process objects concurrently using 'is' pattern matching
            var tasks = new List<Task>();
            
            for (int taskId = 1; taskId <= 4; taskId++)
            {
                int id = taskId;
                tasks.Add(Task.Run(() => ProcessQueueItems(sharedQueue, results, id)));
            }

            await Task.WhenAll(tasks);

            Console.WriteLine("\nProcessing results:");
            foreach (var result in results.OrderBy(kvp => kvp.Key))
            {
                Console.WriteLine($"  {result.Key}: {result.Value} items processed");
            }

            Console.WriteLine("\nThread safety notes:");
            Console.WriteLine("  - 'is' keyword itself is thread-safe (read-only type checking)");
            Console.WriteLine("  - Pattern matching variables are local to the thread");
            Console.WriteLine("  - Shared object references need proper synchronization");
            Console.WriteLine("  - ConcurrentQueue ensures thread-safe queue operations");

            Console.WriteLine();
        }

        static void ProcessQueueItems(ConcurrentQueue<object> queue, ConcurrentDictionary<string, int> results, int taskId)
        {
            var taskName = $"Task{taskId}";
            var localCounts = new Dictionary<string, int>
            {
                ["int"] = 0,
                ["string"] = 0, 
                ["double"] = 0,
                ["other"] = 0
            };

            while (queue.TryDequeue(out var item))
            {
                // MULTITHREAD ASPECTS: 'is' pattern matching is thread-safe
                // Each thread gets its own pattern variable scope
                switch (item)
                {
                    case int intValue:
                        localCounts["int"]++;
                        break;
                    case string stringValue:
                        localCounts["string"]++;
                        break;
                    case double doubleValue:
                        localCounts["double"]++;
                        break;
                    default:
                        localCounts["other"]++;
                        break;
                }

                // Simulate some processing time
                Thread.Sleep(1);
            }

            // Update shared results
            foreach (var count in localCounts)
            {
                var key = $"{taskName}-{count.Key}";
                results.TryAdd(key, count.Value);
            }
        }

        #endregion
    }

    #region Supporting Classes for Examples

    // Base classes for polymorphism example
    public abstract class Shape
    {
        public abstract double CalculateArea();
    }

    public class Circle : Shape
    {
        public double Radius { get; }

        public Circle(double radius)
        {
            Radius = radius;
        }

        public override double CalculateArea()
        {
            return Math.PI * Radius * Radius;
        }
    }

    public class Rectangle : Shape
    {
        public double Width { get; }
        public double Height { get; }

        public Rectangle(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public override double CalculateArea()
        {
            return Width * Height;
        }
    }

    public class Triangle : Shape
    {
        public double SideA { get; }
        public double SideB { get; }
        public double SideC { get; }

        public Triangle(double sideA, double sideB, double sideC)
        {
            SideA = sideA;
            SideB = sideB;
            SideC = sideC;
        }

        public override double CalculateArea()
        {
            // Using Heron's formula
            double s = (SideA + SideB + SideC) / 2;
            return Math.Sqrt(s * (s - SideA) * (s - SideB) * (s - SideC));
        }
    }

    // Animal hierarchy for inheritance example
    public abstract class Animal
    {
        public string Name { get; }

        protected Animal(string name)
        {
            Name = name;
        }

        public abstract void MakeSound();
    }

    public class Dog : Animal
    {
        public string Breed { get; }

        public Dog(string name, string breed) : base(name)
        {
            Breed = breed;
        }

        public override void MakeSound()
        {
            Console.WriteLine("  Woof! Woof!");
        }

        public void Fetch()
        {
            Console.WriteLine("  Dog is fetching the ball!");
        }
    }

    public class Cat : Animal
    {
        public int Lives { get; }

        public Cat(string name, int lives) : base(name)
        {
            Lives = lives;
        }

        public override void MakeSound()
        {
            Console.WriteLine("  Meow! Meow!");
        }

        public void Purr()
        {
            Console.WriteLine("  Cat is purring contentedly...");
        }
    }

    public class Bird : Animal
    {
        public bool CanFly { get; }

        public Bird(string name, bool canFly) : base(name)
        {
            CanFly = canFly;
        }

        public override void MakeSound()
        {
            Console.WriteLine("  Tweet! Tweet!");
        }

        public void Fly()
        {
            if (CanFly)
                Console.WriteLine("  Bird is soaring through the sky!");
            else
                Console.WriteLine("  Bird cannot fly but walks around.");
        }
    }

    public class Fish : Animal
    {
        public string WaterType { get; }

        public Fish(string name, string waterType) : base(name)
        {
            WaterType = waterType;
        }

        public override void MakeSound()
        {
            Console.WriteLine("  Blub! Blub!");
        }

        public void Swim()
        {
            Console.WriteLine($"  Fish is swimming in {WaterType} water!");
        }
    }

    // Person class for advanced pattern matching
    public class Person
    {
        public string Name { get; }
        public int Age { get; }
        public string Job { get; }

        public Person(string name, int age, string job)
        {
            Name = name;
            Age = age;
            Job = job;
        }
    }

    #endregion
}

/*
 * MEMORY ALLOCATION CONSIDERATIONS:
 * =================================
 * 
 * 1. TYPE CHECKING OVERHEAD:
 *    - 'is' keyword performs type metadata comparison
 *    - No object allocation during type checking
 *    - More efficient than try-catch casting approaches
 * 
 * 2. PATTERN MATCHING VARIABLES:
 *    - Pattern variables don't create new allocations
 *    - They're just typed references to existing objects
 *    - Scope is limited to the pattern matching block
 * 
 * 3. BOXING/UNBOXING:
 *    - Value types may be boxed when cast to object
 *    - 'is' pattern matching can help avoid unnecessary boxing
 *    - Use generic constraints when possible to avoid boxing
 * 
 * 4. STRING COMPARISON:
 *    - String pattern matching uses reference equality first
 *    - Falls back to value equality for constant patterns
 *    - Consider string interning for frequently used constants
 * 
 * MULTITHREAD CONSIDERATIONS:
 * ===========================
 * 
 * 1. TYPE CHECKING SAFETY:
 *    - 'is' keyword is inherently thread-safe (read-only operation)
 *    - Type metadata is immutable and shared safely across threads
 *    - No synchronization needed for type checking itself
 * 
 * 2. PATTERN VARIABLE SCOPE:
 *    - Pattern matching variables are local to each thread
 *    - No shared state between threads for pattern variables
 *    - Each thread gets its own variable scope
 * 
 * 3. OBJECT REFERENCE SAFETY:
 *    - The objects being tested may need synchronization
 *    - 'is' only checks type, not object state
 *    - Subsequent property access may need thread safety
 * 
 * 4. RACE CONDITIONS:
 *    - Object type cannot change after creation (immutable)
 *    - Race conditions may occur when accessing object members
 *    - Use appropriate synchronization for shared mutable objects
 * 
 * 5. BEST PRACTICES:
 *    - 'is' pattern matching is preferred over 'as' + null check
 *    - Use switch expressions for complex pattern matching
 *    - Combine with concurrent collections for thread-safe operations
 *    - Consider readonly properties to reduce thread safety concerns
 *    - Use local pattern variables to avoid shared state issues
 */
