# C# Parallelism Guide

## Key Concepts Demonstrated

### 1. **Parallel.For and Parallel.ForEach**
- **Use Case**: CPU-intensive operations on collections or ranges
- **Benefits**: Automatic work distribution across available CPU cores
- **Notice**: Different threads (thread IDs) handle different items

### 2. **Task Parallelism**
- **Use Case**: Independent operations that can run simultaneously
- **Key Methods**:
  - `Task.Run()`: Queues work on thread pool
  - `Task.WaitAll()`: Waits for all tasks to complete
  - `Task.WhenAll()`: Async version of WaitAll

### 3. **PLINQ (Parallel LINQ)**
- **Use Case**: LINQ queries that can benefit from parallelization
- **How to Use**: Add `.AsParallel()` to any IEnumerable
- **Note**: Not always faster for small datasets due to overhead

### 4. **Async/Await**
- **Use Case**: I/O-bound operations (network, file system, database)
- **Benefits**: Non-blocking, doesn't tie up threads while waiting
- **Key**: Use `async Task` and `await` keywords

## When to Use Each Approach

### Use **Parallel.For/ForEach** when:
- Processing large collections
- CPU-intensive operations
- Independent iterations (no shared state)

### Use **Task Parallelism** when:
- Running different independent operations
- Need fine control over task execution
- Combining results from multiple operations

### Use **PLINQ** when:
- Complex LINQ queries on large datasets
- CPU-intensive transformations or filtering
- Want simple parallel syntax

### Use **Async/Await** when:
- I/O operations (web requests, file reads, database calls)
- UI responsiveness is important
- Operations involve waiting for external resources

## Important Considerations

### Thread Safety
```csharp
// ❌ Not thread-safe
int counter = 0;
Parallel.For(0, 1000, i => counter++); // Race condition!

// ✅ Thread-safe
int counter = 0;
Parallel.For(0, 1000, i => Interlocked.Increment(ref counter));
```

### Performance Overhead
- Parallelism has overhead (thread creation, synchronization)
- For small datasets, sequential code might be faster
- Always measure performance in your specific scenario

### CPU vs I/O Bound
- **CPU-bound**: Use Parallel.* or Task.Run()
- **I/O-bound**: Use async/await (don't use Task.Run for I/O)

### Degree of Parallelism
```csharp
// Limit parallel degree
var options = new ParallelOptions
{
    MaxDegreeOfParallelism = 4
};
Parallel.For(0, 100, options, i => DoWork(i));
```

## Common Patterns

### Producer-Consumer with BlockingCollection
```csharp
var collection = new BlockingCollection<int>();

// Producer
Task.Run(() =>
{
    for (int i = 0; i < 100; i++)
    {
        collection.Add(i);
    }
    collection.CompleteAdding();
});

// Consumer
Parallel.ForEach(collection.GetConsumingEnumerable(), item =>
{
    ProcessItem(item);
});
```

### ConfigureAwait(false) for Libraries
```csharp
// In library code
public async Task<string> GetDataAsync()
{
    var result = await SomeAsyncOperation().ConfigureAwait(false);
    return result;
}
```

## Performance Tips

1. **Measure First**: Always profile before optimizing
2. **Right Tool**: Choose the right parallelism approach for your scenario
3. **Avoid Over-Parallelization**: Too many threads can hurt performance
4. **Consider Context**: UI apps vs server apps have different needs
5. **Memory Usage**: Parallel operations can increase memory usage

## Common Pitfalls to Avoid

1. **Shared Mutable State**: Always protect with locks or use thread-safe collections
2. **Blocking Async Code**: Don't use `.Result` or `.Wait()` in async contexts
3. **Wrong Parallelism Type**: Don't use Task.Run for I/O operations
4. **Exception Handling**: Use proper exception handling in parallel code
5. **Resource Contention**: Be careful with shared resources (files, databases)
