using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

// This file demonstrates many ways to use lambda expressions in C#.
// Each example is small, self-contained, and heavily commented so you can
// learn the different patterns: delegates, Func/Action/Predicate, closures,
// LINQ with lambdas, expression trees, async lambdas, and event handlers.

class Program
{
	static async Task Main(string[] args)
	{
		Console.WriteLine("--- Lambda Expressions in C# Examples ---\n");

		// 1) Basic lambda assigned to a delegate
		Console.WriteLine("1) Basic delegate lambda:");
		// We create a delegate that takes an int and returns a string.
		Func<int, string> showNumber = n => $"Number: {n}";
		Console.WriteLine(showNumber(5));
		Console.WriteLine();

		// 2) Action lambda (no return value)
		Console.WriteLine("2) Action (void) lambda:");
		// Action<T> is used for lambdas that return void.
		Action<string> greet = name => Console.WriteLine($"Hello, {name}!");
		greet("Alice");
		Console.WriteLine();

		// 3) Predicate lambda (returns bool)
		Console.WriteLine("3) Predicate lambda:");
		// Predicate<T> is a common pattern for functions that test a condition.
		Predicate<int> isEven = x => x % 2 == 0;
		Console.WriteLine($"4 is even? {isEven(4)}");
		Console.WriteLine();

		// 4) Lambda with multiple statements (block body)
		Console.WriteLine("4) Block-bodied lambda:");
		Func<int, int, int> addAndLog = (a, b) =>
		{
			var sum = a + b;
			Console.WriteLine($"Adding {a} + {b} = {sum}");
			return sum;
		};
		var result = addAndLog(3, 7);
		Console.WriteLine($"Result: {result}");
		Console.WriteLine();

		// 5) Closures: lambdas capturing local variables
		Console.WriteLine("5) Closure example:");
		Func<int> makeCounter = CreateCounter();
		Console.WriteLine(makeCounter()); // 1
		Console.WriteLine(makeCounter()); // 2
		Console.WriteLine(makeCounter()); // 3
		Console.WriteLine();

		// 6) Using lambdas with LINQ
		Console.WriteLine("6) LINQ with lambdas:");
		var numbers = Enumerable.Range(1, 10).ToList();
		// Where and Select both accept lambda expressions
		var evenSquares = numbers
			.Where(n => n % 2 == 0)             // filter
			.Select(n => n * n)                 // transform
			.ToList();                          // execute
		Console.WriteLine($"Even squares: {string.Join(", ", evenSquares)}");
		Console.WriteLine();

		// 7) Lambda ordering and deferred execution with LINQ
		Console.WriteLine("7) Deferred execution demonstration:");
		var query = numbers.Where(n =>
		{
			Console.WriteLine($"Evaluating {n}");
			return n % 2 == 1;
		});
		Console.WriteLine("Query created, not executed yet.");
		Console.WriteLine("Enumerating query:");
		foreach (var n in query)
		{
			Console.WriteLine(n);
		}
		Console.WriteLine();

		// 8) Expression trees: capturing a lambda as data
		Console.WriteLine("8) Expression tree:");
		Expression<Func<int, int, int>> expr = (x, y) => x * y + 2;
		// We can inspect the expression tree, compile it to a delegate, or transform it.
		Console.WriteLine($"Expression body: {expr.Body}");
		var compiled = expr.Compile();
		Console.WriteLine($"compiled(3,4) = {compiled(3, 4)}");
		Console.WriteLine();

		// 9) Async lambdas (Func<Task> / Func<Task<T>>)
		Console.WriteLine("9) Async lambda:");
		Func<int, Task<int>> asyncDouble = async n =>
		{
			await Task.Delay(50); // simulate async work
			return n * 2;
		};
		var doubled = await asyncDouble(21);
		Console.WriteLine($"21 doubled = {doubled}");
		Console.WriteLine();

		// 10) Event handlers using lambdas
		Console.WriteLine("10) Event handler with lambda:");
		var publisher = new SimplePublisher();
		// Subscribe using a lambda - concise and inline
		publisher.Changed += (s, e) => Console.WriteLine($"Received event: {e.Message}");
		publisher.Raise("Hello event");
		Console.WriteLine();

		// 11) Lambdas and variable capture pitfalls (common mistake)
		Console.WriteLine("11) Variable capture pitfall in loops:");
		var actions = new List<Action>();
		for (int i = 0; i < 5; i++)
		{
			// Each lambda captures the variable 'i' (the same storage), so all lambdas
			// will observe the final value of 'i' after the loop unless we capture a copy.
			actions.Add(() => Console.WriteLine($"Captured i = {i}"));
		}
		Console.WriteLine("Invoking actions (expected 5, but might print same value):");
		foreach (var a in actions) a();
		Console.WriteLine("Fixing with local copy:");
		actions.Clear();
		for (int i = 0; i < 5; i++)
		{
			int copy = i; // capture this instead
			actions.Add(() => Console.WriteLine($"Captured copy = {copy}"));
		}
		foreach (var a in actions) a();
		Console.WriteLine();

		// 12) Higher-order functions: returning lambdas
		Console.WriteLine("12) Higher-order function returning a lambda:");
		var multiplyBy3 = MakeMultiplier(3);
		Console.WriteLine($"5 * 3 = {multiplyBy3(5)}");
		Console.WriteLine();

		// 13) Composing lambdas
		Console.WriteLine("13) Composing lambdas:");
		Func<int, int> inc = x => x + 1;
		Func<int, int> sq = x => x * x;
		var incThenSq = Compose(inc, sq); // sq(inc(x))
		Console.WriteLine($"incThenSq(4) = {incThenSq(4)}");
		Console.WriteLine();

		Console.WriteLine("All lambda examples complete.");
	}

	// Helper: create a counter closure
	// Returns a Func<int> that increments and returns an internal counter.
	static Func<int> CreateCounter()
	{
		int count = 0; // captured by the lambda
		return () => ++count; // each call increments the same captured variable
	}

	// Helper: higher-order function that returns a multiplier lambda
	static Func<int, int> MakeMultiplier(int factor)
	{
		// The returned lambda captures 'factor'
		return n => n * factor;
	}

	// Helper: compose two functions f and g into g(f(x))
	static Func<T, V> Compose<T, U, V>(Func<T, U> f, Func<U, V> g)
	{
		return x => g(f(x));
	}
}

// Simple publisher class to demonstrate subscribing with lambda event handlers
class SimplePublisher
{
	public event EventHandler<SimpleEventArgs>? Changed;

	public void Raise(string message)
	{
		Changed?.Invoke(this, new SimpleEventArgs(message));
	}
}

class SimpleEventArgs : EventArgs
{
	public string Message { get; }
	public SimpleEventArgs(string message) => Message = message;
}
