using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Linq;

namespace CollectionDictionary
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Dictionary<TKey,TValue> Comprehensive Examples ===\n");
            
            // Run all example methods
            BasicUsageExample();
            PerformanceAndMemoryExample();
            WhenToUseExample();
            WhenNotToUseExample();
            MultiThreadingExample();
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// PURPOSE: Dictionary<TKey,TValue> is a collection of key-value pairs where:
        /// - Keys must be unique and cannot be null
        /// - Values can be duplicated and can be null (if TValue allows it)
        /// - Provides O(1) average time complexity for lookups, insertions, and deletions
        /// - Uses hash table internally for fast key-based access
        /// </summary>
        static void BasicUsageExample()
        {
            Console.WriteLine("1. BASIC USAGE EXAMPLES");
            Console.WriteLine("========================");
            
            // Creating dictionaries with different initialization methods
            
            // Method 1: Default constructor
            var userAges = new Dictionary<string, int>();
            
            // Method 2: Collection initializer (C# 3.0+)
            var countryCapitals = new Dictionary<string, string>
            {
                {"USA", "Washington D.C."},
                {"France", "Paris"},
                {"Japan", "Tokyo"},
                {"Brazil", "Brasília"}
            };
            
            // Method 3: Dictionary initializer (C# 6.0+)
            var httpStatusCodes = new Dictionary<int, string>
            {
                [200] = "OK",
                [404] = "Not Found",
                [500] = "Internal Server Error"
            };
            
            // Adding elements
            userAges.Add("Alice", 25);
            userAges.Add("Bob", 30);
            userAges["Charlie"] = 35; // Using indexer (safer for updates)
            
            // Accessing elements
            Console.WriteLine($"Alice's age: {userAges["Alice"]}");
            
            // Safe access using TryGetValue (recommended approach)
            if (userAges.TryGetValue("David", out int davidAge))
            {
                Console.WriteLine($"David's age: {davidAge}");
            }
            else
            {
                Console.WriteLine("David not found in the dictionary");
            }
            
            // Checking if key exists
            if (userAges.ContainsKey("Bob"))
            {
                Console.WriteLine($"Bob exists with age: {userAges["Bob"]}");
            }
            
            // Iterating through dictionary
            Console.WriteLine("\nAll users and their ages:");
            foreach (var kvp in userAges)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }
            
            // Working with keys and values separately
            Console.WriteLine($"\nNumber of users: {userAges.Count}");
            Console.WriteLine($"All users: {string.Join(", ", userAges.Keys)}");
            Console.WriteLine($"All ages: {string.Join(", ", userAges.Values)}");
            
            Console.WriteLine();
        }

        /// <summary>
        /// MEMORY ALLOCATION ASPECTS:
        /// - Dictionary allocates memory for internal hash table (buckets array)
        /// - Initial capacity is 0, grows to powers of 2 (4, 8, 16, 32, etc.)
        /// - Each resize operation creates a new array and rehashes all elements
        /// - Memory overhead: ~28 bytes per entry + key/value size on 64-bit systems
        /// - Load factor is maintained around 0.75 to balance performance and memory
        /// </summary>
        static void PerformanceAndMemoryExample()
        {
            Console.WriteLine("2. PERFORMANCE AND MEMORY ALLOCATION");
            Console.WriteLine("====================================");
            
            // Demonstrating capacity and resize behavior
            var dict = new Dictionary<int, string>();
            
            Console.WriteLine("Observing dictionary growth:");
            for (int i = 0; i < 20; i++)
            {
                dict.Add(i, $"Value_{i}");
                // Note: There's no public Capacity property, but we can observe behavior
                if (i % 5 == 0)
                {
                    Console.WriteLine($"Added {i + 1} items, Count: {dict.Count}");
                }
            }
            
            // Pre-sizing dictionary when you know approximate size (memory optimization)
            var preSizedDict = new Dictionary<int, string>(1000);
            Console.WriteLine($"Pre-sized dictionary created with capacity for ~1000 items");
            
            // Performance comparison: Dictionary vs Linear Search
            var largeDict = new Dictionary<int, string>();
            var largeList = new List<KeyValuePair<int, string>>();
            
            const int itemCount = 10000;
            for (int i = 0; i < itemCount; i++)
            {
                largeDict[i] = $"Value_{i}";
                largeList.Add(new KeyValuePair<int, string>(i, $"Value_{i}"));
            }
            
            // Time dictionary lookup (O(1) average)
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
            {
                var value = largeDict[i * 10];
            }
            sw.Stop();
            Console.WriteLine($"Dictionary lookups (1000 operations): {sw.ElapsedTicks} ticks");
            
            // Time list linear search (O(n))
            sw.Restart();
            for (int i = 0; i < 1000; i++)
            {
                var kvp = largeList.FirstOrDefault(x => x.Key == i * 10);
            }
            sw.Stop();
            Console.WriteLine($"List linear search (1000 operations): {sw.ElapsedTicks} ticks");
            
            Console.WriteLine();
        }

        /// <summary>
        /// SCENARIOS TO USE Dictionary<TKey,TValue>:
        /// 1. Fast lookups by unique key (user profiles, caching, configuration)
        /// 2. Counting occurrences (word frequency, character counting)
        /// 3. Mapping relationships (ID to object, enum to string)
        /// 4. Caching computed results
        /// 5. Implementing lookup tables
        /// 6. Grouping data by unique identifier
        /// </summary>
        static void WhenToUseExample()
        {
            Console.WriteLine("3. WHEN TO USE DICTIONARY - GOOD SCENARIOS");
            Console.WriteLine("==========================================");
            
            // Scenario 1: User profile lookup system
            Console.WriteLine("Scenario 1: User Profile Management");
            var userProfiles = new Dictionary<int, UserProfile>
            {
                {1001, new UserProfile("Alice", "alice@email.com", "Admin")},
                {1002, new UserProfile("Bob", "bob@email.com", "User")},
                {1003, new UserProfile("Charlie", "charlie@email.com", "Moderator")}
            };
            
            // Fast O(1) lookup by user ID
            if (userProfiles.TryGetValue(1002, out var bobProfile))
            {
                Console.WriteLine($"Found user: {bobProfile.Name} ({bobProfile.Role})");
            }
            
            // Scenario 2: Word frequency counter
            Console.WriteLine("\nScenario 2: Word Frequency Analysis");
            string text = "the quick brown fox jumps over the lazy dog the fox is quick";
            var wordFrequency = new Dictionary<string, int>();
            
            foreach (string word in text.Split(' '))
            {
                if (wordFrequency.ContainsKey(word))
                    wordFrequency[word]++;
                else
                    wordFrequency[word] = 1;
                    
                // Alternative using TryGetValue (more efficient):
                // wordFrequency[word] = wordFrequency.TryGetValue(word, out int count) ? count + 1 : 1;
            }
            
            foreach (var kvp in wordFrequency.OrderByDescending(x => x.Value))
            {
                Console.WriteLine($"'{kvp.Key}': {kvp.Value} times");
            }
            
            // Scenario 3: Configuration/Settings lookup
            Console.WriteLine("\nScenario 3: Application Configuration");
            var appConfig = new Dictionary<string, object>
            {
                {"DatabaseConnectionString", "Server=localhost;Database=MyApp;"},
                {"MaxRetryAttempts", 3},
                {"EnableLogging", true},
                {"CacheExpirationMinutes", 30}
            };
            
            // Type-safe configuration access
            if (appConfig.TryGetValue("MaxRetryAttempts", out var maxRetries) && maxRetries is int retryCount)
            {
                Console.WriteLine($"Max retry attempts configured: {retryCount}");
            }
            
            Console.WriteLine();
        }

        /// <summary>
        /// SCENARIOS NOT TO USE Dictionary<TKey,TValue>:
        /// 1. When you need ordered data (use SortedDictionary or List instead)
        /// 2. When you have duplicate keys (use Lookup<TKey,TElement> or GroupBy)
        /// 3. When memory is extremely constrained and you have few items
        /// 4. When you need thread-safe operations (use ConcurrentDictionary)
        /// 5. When you need range queries or sorted access patterns
        /// 6. When keys are not suitable for hashing (mutable objects as keys)
        /// </summary>
        static void WhenNotToUseExample()
        {
            Console.WriteLine("4. WHEN NOT TO USE DICTIONARY - PROBLEMATIC SCENARIOS");
            Console.WriteLine("====================================================");
            
            // Problem 1: Ordered data requirement
            Console.WriteLine("Problem 1: Need for ordered data");
            Console.WriteLine("❌ Regular Dictionary doesn't maintain insertion order");
            
            var unorderedDict = new Dictionary<string, int>
            {
                {"First", 1}, {"Second", 2}, {"Third", 3}
            };
            
            Console.WriteLine("Dictionary order (may vary):");
            foreach (var kvp in unorderedDict)
            {
                Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
            }
            
            Console.WriteLine("✅ Use SortedDictionary for sorted order or preserve insertion order manually");
            
            // Problem 2: Duplicate keys scenario
            Console.WriteLine("\nProblem 2: Multiple values per key");
            Console.WriteLine("❌ Dictionary can't handle multiple values for the same key");
            
            // This would throw an exception:
            // dict.Add("Category", "Electronics");
            // dict.Add("Category", "Computers"); // ArgumentException!
            
            Console.WriteLine("✅ Use Lookup<TKey,TElement> or Dictionary<TKey,List<TValue>> instead");
            var categoryProducts = new Dictionary<string, List<string>>
            {
                {"Electronics", new List<string> {"Laptop", "Phone", "Tablet"}},
                {"Books", new List<string> {"C# Guide", "Design Patterns"}}
            };
            
            // Problem 3: Mutable objects as keys (dangerous!)
            Console.WriteLine("\nProblem 3: Mutable objects as keys");
            Console.WriteLine("❌ Using mutable objects as keys can break dictionary");
            
            var mutableKeyDict = new Dictionary<MutableKey, string>();
            var key = new MutableKey { Id = 1, Name = "Original" };
            mutableKeyDict[key] = "Some Value";
            
            Console.WriteLine($"Value found before mutation: {mutableKeyDict.ContainsKey(key)}");
            
            // Mutating the key breaks the dictionary!
            key.Name = "Modified";
            Console.WriteLine($"Value found after mutation: {mutableKeyDict.ContainsKey(key)}");
            Console.WriteLine("✅ Use immutable objects or value types as keys");
            
            Console.WriteLine();
        }

        /// <summary>
        /// MULTITHREADING ASPECTS:
        /// - Dictionary<TKey,TValue> is NOT thread-safe
        /// - Multiple readers can access simultaneously if no writers
        /// - Any write operation requires synchronization
        /// - Use ConcurrentDictionary<TKey,TValue> for thread-safe operations
        /// - Lock-based synchronization possible but reduces performance
        /// - Reader-writer locks can optimize read-heavy scenarios
        /// </summary>
        static void MultiThreadingExample()
        {
            Console.WriteLine("5. MULTITHREADING CONSIDERATIONS");
            Console.WriteLine("=================================");
            
            // Demonstrating thread safety issues with regular Dictionary
            Console.WriteLine("❌ Regular Dictionary is NOT thread-safe:");
            
            var unsafeDict = new Dictionary<int, string>();
            var tasks = new Task[4];
            
            // This could cause race conditions, exceptions, or data corruption
            for (int i = 0; i < 4; i++)
            {
                int taskId = i;
                tasks[i] = Task.Run(() =>
                {
                    try
                    {
                        for (int j = 0; j < 100; j++)
                        {
                            lock (unsafeDict) // Manual synchronization required
                            {
                                unsafeDict[taskId * 100 + j] = $"Task{taskId}_Value{j}";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Task {taskId} failed: {ex.Message}");
                    }
                });
            }
            
            Task.WaitAll(tasks);
            Console.WriteLine($"Unsafe dictionary final count: {unsafeDict.Count}");
            
            // ✅ Using ConcurrentDictionary for thread-safe operations
            Console.WriteLine("\n✅ ConcurrentDictionary is thread-safe:");
            
            var safeDict = new ConcurrentDictionary<int, string>();
            var safeTasks = new Task[4];
            
            for (int i = 0; i < 4; i++)
            {
                int taskId = i;
                safeTasks[i] = Task.Run(() =>
                {
                    for (int j = 0; j < 100; j++)
                    {
                        // No manual synchronization needed
                        safeDict.TryAdd(taskId * 100 + j, $"Task{taskId}_Value{j}");
                    }
                });
            }
            
            Task.WaitAll(safeTasks);
            Console.WriteLine($"Safe dictionary final count: {safeDict.Count}");
            
            // Demonstrating different concurrent operations
            Console.WriteLine("\nConcurrentDictionary advanced operations:");
            
            // AddOrUpdate - adds if key doesn't exist, updates if it does
            var counter = new ConcurrentDictionary<string, int>();
            
            Parallel.For(0, 1000, i =>
            {
                string key = $"Group{i % 10}";
                counter.AddOrUpdate(key, 1, (existingKey, oldValue) => oldValue + 1);
            });
            
            Console.WriteLine("Concurrent counting results:");
            foreach (var kvp in counter.OrderBy(x => x.Key))
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }
            
            // GetOrAdd - gets existing value or adds new one atomically
            var cache = new ConcurrentDictionary<string, string>();
            
            var result1 = cache.GetOrAdd("Key1", key => $"Computed value for {key}");
            var result2 = cache.GetOrAdd("Key1", key => $"This won't be called");
            
            Console.WriteLine($"\nCache result 1: {result1}");
            Console.WriteLine($"Cache result 2: {result2}");
            Console.WriteLine($"Results are same: {result1 == result2}");
            
            Console.WriteLine();
        }
    }

    // Supporting classes for examples
    public class UserProfile
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        
        public UserProfile(string name, string email, string role)
        {
            Name = name;
            Email = email;
            Role = role;
        }
    }
    
    // Example of a mutable key (problematic for Dictionary keys)
    public class MutableKey
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }
        
        public override bool Equals(object? obj)
        {
            return obj is MutableKey other && Id == other.Id && Name == other.Name;
        }
    }
}
