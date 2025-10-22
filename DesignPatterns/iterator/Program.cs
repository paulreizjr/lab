using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/*
 * ITERATOR DESIGN PATTERN IN C#
 * 
 * PURPOSE:
 * The Iterator pattern provides a way to access elements of a collection sequentially
 * without exposing the underlying representation. It decouples algorithms from data structures.
 * 
 * CORE BENEFITS:
 * - Uniform interface for traversing different data structures
 * - Single responsibility: iteration logic is separated from business logic
 * - Support for multiple simultaneous traversals of the same structure
 * - Lazy evaluation support (compute elements on-demand)
 * 
 * SCENARIOS TO USE:
 * - When you need to traverse a complex data structure without exposing its internals
 * - When you want to provide multiple ways to traverse the same data structure
 * - When implementing lazy evaluation or streaming data processing
 * - When you need to support foreach loops on custom collections
 * - When working with large datasets that shouldn't be loaded entirely into memory
 * 
 * SCENARIOS NOT TO USE:
 * - Simple arrays or lists where built-in iteration is sufficient
 * - When performance is critical and iterator overhead is unacceptable
 * - Very small collections where the pattern adds unnecessary complexity
 * - When you need random access to elements (use indexers instead)
 * - When the collection structure changes frequently during iteration
 */

namespace IteratorPattern
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Iterator Design Pattern Examples ===\n");

            // Example 1: Basic Iterator Implementation
            BasicIteratorExample();

            // Example 2: Generic Iterator with IEnumerable<T>
            GenericIteratorExample();

            // Example 3: Lazy Evaluation Iterator
            LazyEvaluationExample();

            // Example 4: Custom Tree Iterator
            TreeIteratorExample();

            // Example 5: Thread-Safety Considerations
            await ThreadSafetyExample();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        #region Example 1: Basic Iterator Implementation

        static void BasicIteratorExample()
        {
            Console.WriteLine("1. Basic Iterator Implementation:");
            Console.WriteLine("================================");

            var bookCollection = new BookCollection();
            bookCollection.AddBook(new Book("The Pragmatic Programmer", "Hunt & Thomas"));
            bookCollection.AddBook(new Book("Clean Code", "Robert Martin"));
            bookCollection.AddBook(new Book("Design Patterns", "Gang of Four"));

            // Using the iterator
            Console.WriteLine("Books in collection:");
            var iterator = bookCollection.GetIterator();
            while (iterator.HasNext())
            {
                var book = iterator.Next();
                Console.WriteLine($"- {book.Title} by {book.Author}");
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 2: Generic Iterator with IEnumerable<T>

        static void GenericIteratorExample()
        {
            Console.WriteLine("2. Generic Iterator with IEnumerable<T>:");
            Console.WriteLine("==========================================");

            var numberSequence = new FibonacciSequence(10);

            Console.WriteLine("Fibonacci sequence (first 10 numbers):");
            foreach (var number in numberSequence)
            {
                Console.WriteLine($"- {number}");
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 3: Lazy Evaluation Iterator

        static void LazyEvaluationExample()
        {
            Console.WriteLine("3. Lazy Evaluation Iterator:");
            Console.WriteLine("=============================");

            Console.WriteLine("Generating squares lazily (only first 5 will be computed):");
            
            // This demonstrates lazy evaluation - squares are computed only when requested
            var lazySquares = GenerateSquares(1000000); // Large number, but only computed on demand
            
            var count = 0;
            foreach (var square in lazySquares)
            {
                Console.WriteLine($"- Square: {square}");
                if (++count >= 5) break; // Only compute first 5
            }

            Console.WriteLine();
        }

        // Lazy iterator using yield return
        // MEMORY ALLOCATION: Uses minimal memory as values are generated on-demand
        static IEnumerable<int> GenerateSquares(int maxCount)
        {
            Console.WriteLine("  [Generator started - this proves lazy evaluation]");
            for (int i = 1; i <= maxCount; i++)
            {
                Console.WriteLine($"  [Computing square of {i}]");
                yield return i * i; // Computed only when MoveNext() is called
            }
        }

        #endregion

        #region Example 4: Custom Tree Iterator

        static void TreeIteratorExample()
        {
            Console.WriteLine("4. Custom Tree Iterator (In-Order Traversal):");
            Console.WriteLine("===============================================");

            var tree = new BinaryTree<int>();
            tree.Insert(50);
            tree.Insert(30);
            tree.Insert(70);
            tree.Insert(20);
            tree.Insert(40);
            tree.Insert(60);
            tree.Insert(80);

            Console.WriteLine("Tree values (in-order traversal):");
            foreach (var value in tree)
            {
                Console.WriteLine($"- {value}");
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 5: Thread-Safety Considerations

        static async Task ThreadSafetyExample()
        {
            Console.WriteLine("5. Thread-Safety Considerations:");
            Console.WriteLine("=================================");

            var threadSafeCollection = new ThreadSafeCollection<string>();
            
            // Add some initial data
            threadSafeCollection.Add("Item 1");
            threadSafeCollection.Add("Item 2");
            threadSafeCollection.Add("Item 3");

            // Simulate concurrent access
            var tasks = new List<Task>();

            // Task 1: Add items while iteration might be happening
            tasks.Add(Task.Run(() =>
            {
                for (int i = 4; i <= 6; i++)
                {
                    Thread.Sleep(100);
                    threadSafeCollection.Add($"Item {i}");
                    Console.WriteLine($"  [Added Item {i}]");
                }
            }));

            // Task 2: Iterate through collection
            tasks.Add(Task.Run(() =>
            {
                Thread.Sleep(50); // Start slightly after the first task
                Console.WriteLine("Iterating through thread-safe collection:");
                foreach (var item in threadSafeCollection.GetSafeIterator())
                {
                    Console.WriteLine($"- {item}");
                    Thread.Sleep(150); // Slow iteration to allow concurrent modifications
                }
            }));

            await Task.WhenAll(tasks);
            Console.WriteLine();
        }

        #endregion
    }

    #region Supporting Classes for Examples

    // Basic Book class
    public class Book
    {
        public string Title { get; }
        public string Author { get; }

        public Book(string title, string author)
        {
            Title = title;
            Author = author;
        }
    }

    // Basic Iterator Interface (non-generic)
    public interface IIterator<T>
    {
        bool HasNext();
        T Next();
    }

    // Basic Collection Interface
    public interface IAggregate<T>
    {
        IIterator<T> GetIterator();
    }

    // Concrete Iterator Implementation
    public class BookIterator : IIterator<Book>
    {
        private readonly List<Book> _books;
        private int _position = -1;

        public BookIterator(List<Book> books)
        {
            _books = books ?? throw new ArgumentNullException(nameof(books));
        }

        public bool HasNext()
        {
            return _position + 1 < _books.Count;
        }

        public Book Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more elements");
            
            return _books[++_position];
        }
    }

    // Concrete Collection Implementation
    public class BookCollection : IAggregate<Book>
    {
        private readonly List<Book> _books = new List<Book>();

        public void AddBook(Book book)
        {
            _books.Add(book ?? throw new ArgumentNullException(nameof(book)));
        }

        public IIterator<Book> GetIterator()
        {
            return new BookIterator(_books);
        }
    }

    // Generic Collection using IEnumerable<T>
    // MEMORY ALLOCATION: More efficient as it leverages .NET's optimized enumeration
    public class FibonacciSequence : IEnumerable<int>
    {
        private readonly int _count;

        public FibonacciSequence(int count)
        {
            _count = count;
        }

        public IEnumerator<int> GetEnumerator()
        {
            return GenerateFibonacci().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<int> GenerateFibonacci()
        {
            int a = 0, b = 1;
            
            for (int i = 0; i < _count; i++)
            {
                yield return a;
                var temp = a + b;
                a = b;
                b = temp;
            }
        }
    }

    // Binary Tree with Custom Iterator
    // MEMORY ALLOCATION: Uses stack-based iteration to avoid recursion overhead
    public class BinaryTree<T> : IEnumerable<T> where T : IComparable<T>
    {
        private TreeNode<T>? _root;

        public void Insert(T value)
        {
            _root = Insert(_root, value);
        }

        private TreeNode<T>? Insert(TreeNode<T>? node, T value)
        {
            if (node == null)
                return new TreeNode<T>(value);

            if (value.CompareTo(node.Value) < 0)
                node.Left = Insert(node.Left, value);
            else if (value.CompareTo(node.Value) > 0)
                node.Right = Insert(node.Right, value);

            return node;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return InOrderTraversal().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // In-order traversal using explicit stack (better memory control than recursion)
        private IEnumerable<T> InOrderTraversal()
        {
            if (_root == null)
                yield break;

            var stack = new Stack<TreeNode<T>>();
            TreeNode<T>? current = _root;

            while (stack.Count > 0 || current != null)
            {
                // Go to the leftmost node
                while (current != null)
                {
                    stack.Push(current);
                    current = current.Left;
                }

                // Current is null, pop from stack
                current = stack.Pop();
                yield return current.Value;

                // Visit right subtree
                current = current.Right;
            }
        }
    }

    public class TreeNode<T>
    {
        public T Value { get; set; }
        public TreeNode<T>? Left { get; set; }
        public TreeNode<T>? Right { get; set; }

        public TreeNode(T value)
        {
            Value = value;
        }
    }

    // Thread-Safe Collection Example
    // MULTITHREAD ASPECTS: Uses locking and snapshots to ensure safe iteration
    public class ThreadSafeCollection<T>
    {
        private readonly List<T> _items = new List<T>();
        private readonly object _lock = new object();

        public void Add(T item)
        {
            lock (_lock)
            {
                _items.Add(item);
            }
        }

        // Returns a snapshot iterator to avoid concurrent modification issues
        // MEMORY ALLOCATION: Creates a copy of the collection for safe iteration
        public IEnumerable<T> GetSafeIterator()
        {
            List<T> snapshot;
            lock (_lock)
            {
                snapshot = new List<T>(_items); // Create snapshot to avoid concurrent modifications
            }

            // Iterate over snapshot (safe from concurrent modifications)
            foreach (var item in snapshot)
            {
                yield return item;
            }
        }

        // Alternative: Use concurrent collections for better performance
        // For production code, consider using ConcurrentBag<T>, ConcurrentQueue<T>, etc.
    }

    #endregion
}

/*
 * MEMORY ALLOCATION CONSIDERATIONS:
 * =================================
 * 
 * 1. YIELD RETURN ITERATORS:
 *    - Use minimal memory as they generate values on-demand
 *    - State machine is created by compiler (small memory footprint)
 *    - Excellent for large datasets or infinite sequences
 * 
 * 2. COLLECTION SNAPSHOTS:
 *    - Thread-safe but memory-intensive for large collections
 *    - Creates full copy of data at iterator creation time
 *    - Consider for small collections or when consistency is critical
 * 
 * 3. STACK-BASED TRAVERSAL:
 *    - More memory-efficient than recursive approaches
 *    - Stack size grows with tree depth, not total number of nodes
 *    - Suitable for balanced trees with reasonable depth
 * 
 * 4. LAZY EVALUATION:
 *    - Defers computation until values are actually needed
 *    - Can significantly reduce memory usage for filtered operations
 *    - Combine with LINQ for powerful data processing pipelines
 * 
 * MULTITHREAD CONSIDERATIONS:
 * ===========================
 * 
 * 1. CONCURRENT MODIFICATION:
 *    - Standard iterators throw InvalidOperationException if collection changes
 *    - Use concurrent collections (ConcurrentBag, ConcurrentQueue) for thread-safety
 *    - Create snapshots for consistent iteration in multi-threaded scenarios
 * 
 * 2. YIELD RETURN AND THREADING:
 *    - Yield iterators are NOT thread-safe by default
 *    - Multiple threads can safely read the same iterator instance
 *    - Avoid modifying iterator state from multiple threads
 * 
 * 3. PERFORMANCE IMPLICATIONS:
 *    - Locking can create bottlenecks in high-concurrency scenarios
 *    - Consider lock-free data structures for better performance
 *    - Use ThreadLocal<T> for thread-specific iterator state when needed
 * 
 * 4. BEST PRACTICES:
 *    - Prefer immutable collections for thread-safe iteration
 *    - Use concurrent collections from System.Collections.Concurrent
 *    - Document thread-safety guarantees clearly in your APIs
 *    - Consider using async iterators (IAsyncEnumerable) for I/O-bound operations
 */
