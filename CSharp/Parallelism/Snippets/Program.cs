using System.Diagnostics;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== C# Parallelism Examples ===\n");
        
        // Example 1: Parallel.For
        ParallelForExample();
        
        // Example 2: Parallel.ForEach
        ParallelForEachExample();
        
        // Example 3: Task Parallelism
        TaskParallelismExample();
        
        // Example 4: PLINQ
        PlinqExample();
        
        // Example 5: Async/Await
        AsyncAwaitExample().Wait();
        
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    // Example 1: Parallel.For - Processing a range of numbers
    static void ParallelForExample()
    {
        Console.WriteLine("1. Parallel.For Example");
        Console.WriteLine("Processing numbers 1-10 in parallel:");
        
        var sw = Stopwatch.StartNew();
        
        Parallel.For(1, 11, i =>
        {
            // Simulate some work
            Thread.Sleep(100);
            Console.WriteLine($"Processing {i} on thread {Thread.CurrentThread.ManagedThreadId}");
        });
        
        sw.Stop();
        Console.WriteLine($"Parallel.For completed in {sw.ElapsedMilliseconds}ms\n");
    }

    // Example 2: Parallel.ForEach - Processing a collection
    static void ParallelForEachExample()
    {
        Console.WriteLine("2. Parallel.ForEach Example");
        
        var items = new[] { "Apple", "Banana", "Cherry", "Date", "Elderberry" };
        Console.WriteLine("Processing fruits in parallel:");
        
        Parallel.ForEach(items, item =>
        {
            // Simulate some work
            Thread.Sleep(200);
            var processedItem = item.ToUpper();
            Console.WriteLine($"Processed: {processedItem} on thread {Thread.CurrentThread.ManagedThreadId}");
        });
        
        Console.WriteLine("Parallel.ForEach completed\n");
    }

    // Example 3: Task Parallelism
    static void TaskParallelismExample()
    {
        Console.WriteLine("3. Task Parallelism Example");
        Console.WriteLine("Running multiple tasks concurrently:");
        
        var sw = Stopwatch.StartNew();
        
        // Create multiple tasks
        var task1 = Task.Run(() => PerformWork("Task 1", 300));
        var task2 = Task.Run(() => PerformWork("Task 2", 400));
        var task3 = Task.Run(() => PerformWork("Task 3", 200));
        
        // Wait for all tasks to complete
        Task.WaitAll(task1, task2, task3);
        
        sw.Stop();
        Console.WriteLine($"All tasks completed in {sw.ElapsedMilliseconds}ms\n");
    }

    static void PerformWork(string taskName, int delay)
    {
        Console.WriteLine($"{taskName} started on thread {Thread.CurrentThread.ManagedThreadId}");
        Thread.Sleep(delay);
        Console.WriteLine($"{taskName} completed");
    }

    // Example 4: PLINQ (Parallel LINQ)
    static void PlinqExample()
    {
        Console.WriteLine("4. PLINQ Example");
        
        var numbers = Enumerable.Range(1, 1000).ToArray();
        
        // Sequential LINQ
        var sw = Stopwatch.StartNew();
        var sequentialResult = numbers
            .Where(n => IsPrime(n))
            .Count();
        sw.Stop();
        var sequentialTime = sw.ElapsedMilliseconds;
        
        // Parallel LINQ
        sw.Restart();
        var parallelResult = numbers
            .AsParallel()
            .Where(n => IsPrime(n))
            .Count();
        sw.Stop();
        var parallelTime = sw.ElapsedMilliseconds;
        
        Console.WriteLine($"Found {sequentialResult} prime numbers");
        Console.WriteLine($"Sequential time: {sequentialTime}ms");
        Console.WriteLine($"Parallel time: {parallelTime}ms");
        Console.WriteLine($"Speedup: {(double)sequentialTime / parallelTime:F2}x\n");
    }

    static bool IsPrime(int number)
    {
        if (number < 2) return false;
        for (int i = 2; i <= Math.Sqrt(number); i++)
        {
            if (number % i == 0) return false;
        }
        return true;
    }

    // Example 5: Async/Await Pattern
    static async Task AsyncAwaitExample()
    {
        Console.WriteLine("5. Async/Await Example");
        Console.WriteLine("Downloading data asynchronously:");
        
        var sw = Stopwatch.StartNew();
        
        // Start multiple async operations
        var download1 = DownloadDataAsync("Server 1", 500);
        var download2 = DownloadDataAsync("Server 2", 300);
        var download3 = DownloadDataAsync("Server 3", 700);
        
        // Wait for all downloads to complete
        var results = await Task.WhenAll(download1, download2, download3);
        
        sw.Stop();
        
        foreach (var result in results)
        {
            Console.WriteLine(result);
        }
        
        Console.WriteLine($"All downloads completed in {sw.ElapsedMilliseconds}ms");
    }

    static async Task<string> DownloadDataAsync(string server, int delay)
    {
        Console.WriteLine($"Starting download from {server}...");
        await Task.Delay(delay); // Simulate network delay
        return $"Data from {server} downloaded successfully";
    }
}
