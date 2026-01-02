using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace GenericsDemo
{
    /// <summary>
    /// PURPOSE OF GENERICS:
    /// - Type safety at compile time (no casting required)
    /// - Code reusability without sacrificing performance
    /// - Elimination of boxing/unboxing for value types
    /// - Better IntelliSense and debugging experience
    /// 
    /// MEMORY ALLOCATION:
    /// - Generic types are instantiated per unique type parameter at runtime
    /// - Value types in generics avoid boxing (no heap allocation for the value itself)
    /// - Reference types work the same as non-generic equivalents
    /// - Each unique generic type gets its own static fields and methods
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== C# GENERICS DEMONSTRATION ===\n");

            // Basic generic class usage
            DemoBasicGenericClass();
            
            // Generic methods
            DemoGenericMethods();
            
            // Generic collections and performance
            DemoGenericCollections();
            
            // Generic constraints
            DemoGenericConstraints();
            
            // Thread safety with generics
            await DemoThreadSafety();
            
            Console.ReadKey();
        }

        static void DemoBasicGenericClass()
        {
            Console.WriteLine("1. BASIC GENERIC CLASS DEMO");
            Console.WriteLine("SCENARIO TO USE: When you need type-safe containers or operations");
            
            // Generic stack - type safe, no boxing for value types
            var intStack = new GenericStack<int>();
            intStack.Push(10);
            intStack.Push(20);
            
            var stringStack = new GenericStack<string>();
            stringStack.Push("Hello");
            stringStack.Push("World");
            
            Console.WriteLine($"Int stack top: {intStack.Pop()}"); // No casting needed!
            Console.WriteLine($"String stack top: {stringStack.Pop()}");
            Console.WriteLine();
        }

        static void DemoGenericMethods()
        {
            Console.WriteLine("2. GENERIC METHODS DEMO");
            Console.WriteLine("SCENARIO TO USE: When algorithm is the same regardless of type");
            
            // Generic method - works with any type
            Swap(ref intStack, ref anotherIntStack);
            string str1 = "first", str2 = "second";
            Swap(ref str1, ref str2);
            
            Console.WriteLine($"After swap: str1={str1}, str2={str2}");
            Console.WriteLine();
        }

        static void DemoGenericCollections()
        {
            Console.WriteLine("3. GENERIC COLLECTIONS & PERFORMANCE");
            Console.WriteLine("MEMORY BENEFIT: No boxing for value types, better performance");
            
            // Generic List<T> vs non-generic ArrayList
            var genericList = new List<int> { 1, 2, 3, 4, 5 };
            // ArrayList would box each int, causing heap allocation and GC pressure
            
            foreach (int number in genericList)
            {
                // No unboxing cast needed - direct access to int
                Console.WriteLine($"Number: {number}");
            }
            Console.WriteLine();
        }

        static void DemoGenericConstraints()
        {
            Console.WriteLine("4. GENERIC CONSTRAINTS DEMO");
            Console.WriteLine("SCENARIO TO USE: When you need specific capabilities from type parameter");
            
            var calculator = new ConstrainedCalculator<decimal>();
            Console.WriteLine($"Sum: {calculator.Add(10.5m, 20.3m)}");
            
            var processor = new ReferenceTypeProcessor<Person>();
            var person = new Person { Name = "John", Age = 30 };
            processor.Process(person);
            Console.WriteLine();
        }

        static async Task DemoThreadSafety()
        {
            Console.WriteLine("5. MULTITHREADING WITH GENERICS");
            Console.WriteLine("THREAD SAFETY: Generic collections are NOT thread-safe by default");
            Console.WriteLine("Use ConcurrentCollection<T> for thread-safe operations");
            
            var threadSafeQueue = new ConcurrentQueue<string>();
            
            // Multiple tasks adding to queue concurrently
            var tasks = new List<Task>();
            for (int i = 0; i < 5; i++)
            {
                int taskId = i;
                tasks.Add(Task.Run(() =>
                {
                    threadSafeQueue.Enqueue($"Message from task {taskId}");
                }));
            }
            
            await Task.WhenAll(tasks);
            
            Console.WriteLine("Messages from concurrent tasks:");
            while (threadSafeQueue.TryDequeue(out string message))
            {
                Console.WriteLine($"- {message}");
            }
            Console.WriteLine();
        }

        // Generic method example
        static void Swap<T>(ref T first, ref T second)
        {
            T temp = first;
            first = second;
            second = temp;
        }
        
        // For demo purposes
        static GenericStack<int> intStack = new GenericStack<int>();
        static GenericStack<int> anotherIntStack = new GenericStack<int>();
    }

    /// <summary>
    /// GENERIC CLASS EXAMPLE
    /// PURPOSE: Type-safe stack implementation without boxing
    /// MEMORY: Each T instance uses appropriate memory (no boxing for value types)
    /// </summary>
    public class GenericStack<T>
    {
        private readonly List<T> _items = new List<T>();
        
        public void Push(T item)
        {
            // THREAD SAFETY NOTE: This is NOT thread-safe
            // Multiple threads calling Push() simultaneously could cause issues
            _items.Add(item);
        }
        
        public T Pop()
        {
            if (_items.Count == 0)
                throw new InvalidOperationException("Stack is empty");
                
            T item = _items[_items.Count - 1];
            _items.RemoveAt(_items.Count - 1);
            return item; // No casting required!
        }
        
        public int Count => _items.Count;
    }

    /// <summary>
    /// GENERIC CONSTRAINTS EXAMPLE
    /// SCENARIO TO USE: When you need arithmetic operations
    /// CONSTRAINT: where T : struct, IComparable<T> ensures value type with comparison
    /// </summary>
    public class ConstrainedCalculator<T> where T : struct, IComparable<T>
    {
        public T Add(T a, T b)
        {
            // Note: C# doesn't have arithmetic constraints built-in
            // This is a simplified example - real implementation would need
            // more complex solutions like dynamic or expression trees
            return (dynamic)a + (dynamic)b;
        }
    }

    /// <summary>
    /// REFERENCE TYPE CONSTRAINT EXAMPLE
    /// SCENARIO TO USE: When you need reference type guarantees (can be null, inheritance)
    /// CONSTRAINT: where T : class ensures reference type
    /// </summary>
    public class ReferenceTypeProcessor<T> where T : class
    {
        public void Process(T item)
        {
            if (item == null) // Only possible because T : class
            {
                Console.WriteLine("Item is null");
                return;
            }
            
            Console.WriteLine($"Processing: {item.ToString()}");
        }
    }

    /// <summary>
    /// SCENARIOS NOT TO USE GENERICS:
    /// 1. When type-specific logic is required (different behavior per type)
    /// 2. When working with legacy code that expects specific types
    /// 3. When the overhead of generic type instantiation outweighs benefits (rare)
    /// 4. When you need covariance/contravariance with delegates (use interfaces instead)
    /// 
    /// MEMORY CONSIDERATIONS:
    /// - Each unique generic instantiation creates a new type at runtime
    /// - Generic<int> and Generic<string> are completely different types
    /// - Static fields are separate for each instantiation
    /// - JIT optimizations apply per instantiation
    /// </summary>
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        
        public override string ToString()
        {
            return $"{Name} ({Age} years old)";
        }
    }

    /// <summary>
    /// THREAD-SAFE GENERIC WRAPPER EXAMPLE
    /// PURPOSE: Demonstrates how to make generic types thread-safe
    /// MEMORY: Additional overhead for locking mechanism
    /// </summary>
    public class ThreadSafeGenericContainer<T>
    {
        private readonly List<T> _items = new List<T>();
        private readonly object _lock = new object();
        
        public void Add(T item)
        {
            lock (_lock) // Ensures thread safety
            {
                _items.Add(item);
            }
        }
        
        public bool TryRemove(out T item)
        {
            lock (_lock)
            {
                if (_items.Count > 0)
                {
                    item = _items[_items.Count - 1];
                    _items.RemoveAt(_items.Count - 1);
                    return true;
                }
                item = default(T);
                return false;
            }
        }
        
        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _items.Count;
                }
            }
        }
    }
}

/*
SUMMARY OF GENERICS BEST PRACTICES:

WHEN TO USE:
✓ Collections and data structures
✓ Algorithms that work with any type
✓ APIs that need type safety without performance cost
✓ When you want to avoid boxing/unboxing

WHEN NOT TO USE:
✗ Type-specific business logic
✗ When you need different behavior per type
✗ Legacy interop scenarios
✗ When variance is more important than type safety

MEMORY IMPLICATIONS:
- Generic types are instantiated once per unique type parameter
- Value types avoid boxing overhead
- Reference types have same memory characteristics as non-generic equivalents
- Static members are separate per generic instantiation

THREADING CONSIDERATIONS:
- Generic collections are NOT thread-safe by default
- Use concurrent collections (ConcurrentQueue<T>, etc.) for thread safety
- Generic type instantiation is thread-safe (done by CLR)
- Instance members need explicit synchronization
*/
