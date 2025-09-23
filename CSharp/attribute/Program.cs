using System;
using System.Linq;
using System.Reflection;

// This file demonstrates how to define, apply, and read custom attributes in C#.
// It also explains where attribute data lives and notes about memory/allocation.

// High-level notes about attributes and memory:
// - Attributes are metadata attached to assemblies/types/members and stored in
//   the assembly's metadata (not as regular heap objects at compile time).
// - When you call reflection APIs like GetCustomAttributes, attribute instances
//   are instantiated at runtime (heap-allocated) if requested. Calling reflection
//   has runtime cost; cache results when necessary.
// - Attributes themselves should be small and immutable data containers.

// Example 1: Define a simple attribute with a positional and a named argument
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
sealed class DocumentationAttribute : Attribute
{
    // Positional argument (constructor)
    public DocumentationAttribute(string description)
    {
        Description = description;
    }

    // Named argument (property)
    public string Description { get; }
    public string? Author { get; set; }
    public int Version { get; set; }
}

// Example 2: Apply the custom attribute to a class and members
[Documentation("Represents a sample service", Author = "Alice", Version = 1)]
class SampleService
{
    [Documentation("Performs an action", Author = "Bob", Version = 2)]
    public void DoWork() { }

    [Obsolete("Use DoWorkAsync instead")] // built-in attribute example
    public void DoOldWork() { }

    [System.Diagnostics.Conditional("DEBUG")]
    public void DebugOnlyMethod() => Console.WriteLine("DebugOnlyMethod invoked");
}

// Example 3: Attribute with AllowMultiple
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
class TagAttribute : Attribute
{
    public string Name { get; }
    public TagAttribute(string name) => Name = name;
}

[Tag("utility")]
[Tag("experimental")]
class TaggedClass { }

class Program
{
    static void Main()
    {
        Console.WriteLine("--- Attribute examples and memory notes ---\n");

        // Read attribute data via reflection (type-level)
        var t = typeof(SampleService);
        Console.WriteLine($"Attributes on {t.Name}:");

        // GetCustomAttributes creates attribute instances at runtime (heap allocations)
        var attrs = t.GetCustomAttributes(inherit: true).ToArray();
        foreach (var a in attrs)
        {
            Console.WriteLine($" - {a.GetType().Name}");
        }
        Console.WriteLine();

        // Read a specific attribute strongly-typed
        var doc = t.GetCustomAttribute<DocumentationAttribute>(inherit: true);
        if (doc != null)
        {
            Console.WriteLine($"Documentation: {doc.Description}, Author={doc.Author}, Version={doc.Version}");
        }
        Console.WriteLine();

        // Member-level attributes
        var method = t.GetMethod("DoWork");
        var mdoc = method?.GetCustomAttribute<DocumentationAttribute>();
        Console.WriteLine($"DoWork Documentation: {mdoc?.Description} (Author={mdoc?.Author})");
        Console.WriteLine();

        // Demonstrate AllowMultiple on TaggedClass
        var tags = typeof(TaggedClass).GetCustomAttributes<TagAttribute>();
        Console.WriteLine("Tags on TaggedClass: " + string.Join(", ", tags.Select(x => x.Name)));
        Console.WriteLine();

        // Conditional attribute behavior: DebugOnlyMethod only runs when DEBUG is defined
        var svc = new SampleService();
        svc.DebugOnlyMethod(); // may or may not run depending on build symbols
        Console.WriteLine();

        // Memory/allocation notes demonstration: calling GetCustomAttributes creates instances
        Console.WriteLine("Allocation demonstration: retrieving attributes multiple times:");
        var firstCall = t.GetCustomAttributes(inherit: true).ToArray();
        var secondCall = t.GetCustomAttributes(inherit: true).ToArray();
        Console.WriteLine($"First call instances: {firstCall.Length}, second call instances: {secondCall.Length}");
        Console.WriteLine("Note: these are separate object instances; cache attribute data if you need to avoid allocations.");
        Console.WriteLine();

        // Practical guidance
        Console.WriteLine("Practical guidance:");
        Console.WriteLine("- Keep attributes small and immutable; prefer simple data like strings/ints/enums.");
        Console.WriteLine("- Avoid heavy initialization in attribute constructors (they run when instantiated via reflection).");
        Console.WriteLine("- Cache reflection results (attributes) if used repeatedly at runtime.");
        Console.WriteLine("- Use ConditionalAttribute for methods you only want invoked in certain builds (no reflection needed).");
        Console.WriteLine();

        Console.WriteLine("All attribute examples complete.");
    }
}
