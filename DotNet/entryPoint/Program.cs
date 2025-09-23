/*
 * .NET Program Entry Points - Comprehensive Examples
 * 
 * This file demonstrates various ways to define program entry points in .NET
 * Each example shows different approaches and their use cases
 * 
 * Author: Generated Example
 * Date: September 23, 2025
 */

using System;
using System.Threading.Tasks;

namespace EntryPointExamples
{
    /// <summary>
    /// This class demonstrates various .NET program entry point patterns
    /// </summary>
    class Program
    {
        // ==============================================================================
        // EXAMPLE 1: Traditional Main Method (Classic Approach)
        // ==============================================================================
        
        /// <summary>
        /// Traditional Main method - the classic entry point for .NET applications
        /// This is the most common and widely recognized pattern
        /// </summary>
        /// <param name="args">Command line arguments passed to the application</param>
        /// <returns>Exit code (0 for success, non-zero for error)</returns>
        static int Main(string[] args)
        {
            Console.WriteLine("=== Traditional Main Method Entry Point ===");
            
            // Display application startup information
            Console.WriteLine($"Application started at: {DateTime.Now}");
            Console.WriteLine($"Number of command line arguments: {args.Length}");
            
            // Process command line arguments if any were provided
            if (args.Length > 0)
            {
                Console.WriteLine("Command line arguments:");
                for (int i = 0; i < args.Length; i++)
                {
                    Console.WriteLine($"  Arg[{i}]: {args[i]}");
                }
            }
            else
            {
                Console.WriteLine("No command line arguments provided");
            }
            
            // Demonstrate basic program logic
            ProcessMainLogic();
            
            Console.WriteLine("Application completed successfully");
            
            // Return 0 to indicate successful execution
            // Non-zero values typically indicate errors
            return 0;
        }

        // ==============================================================================
        // EXAMPLE 2: Async Main Method (For Async Operations)
        // ==============================================================================
        
        /// <summary>
        /// Async Main method - used when your application needs to perform asynchronous operations
        /// Available since C# 7.1 and .NET Framework 4.6.1+
        /// Uncomment this method and comment out the traditional Main to use this approach
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>Task representing the async operation, with exit code</returns>
        /*
        static async Task<int> Main(string[] args)
        {
            Console.WriteLine("=== Async Main Method Entry Point ===");
            
            try
            {
                // Simulate async initialization (e.g., loading configuration, connecting to services)
                Console.WriteLine("Initializing application asynchronously...");
                await InitializeApplicationAsync();
                
                // Process command line arguments
                await ProcessArgumentsAsync(args);
                
                // Perform main application work
                await PerformAsyncWork();
                
                Console.WriteLine("Async application completed successfully");
                return 0; // Success
            }
            catch (Exception ex)
            {
                // Handle any unhandled exceptions
                Console.WriteLine($"Application error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return 1; // Error exit code
            }
        }
        */

        // ==============================================================================
        // EXAMPLE 3: Top-Level Programs (C# 9+ Minimal Approach)
        // ==============================================================================
        
        /*
         * TOP-LEVEL PROGRAM EXAMPLE (Create a separate file for this)
         * 
         * Since C# 9.0, you can use top-level programs for simpler syntax.
         * This approach eliminates the need for a class and Main method.
         * 
         * Example content for a minimal Program.cs:
         * 
         * // Top-level program - no class or Main method needed
         * using System;
         * 
         * // This is the entry point - code executes from top to bottom
         * Console.WriteLine("=== Top-Level Program Entry Point ===");
         * 
         * // Access command line arguments through the implicit 'args' variable
         * Console.WriteLine($"Arguments count: {args.Length}");
         * foreach (var arg in args)
         * {
         *     Console.WriteLine($"Argument: {arg}");
         * }
         * 
         * // You can still define methods and classes below the top-level statements
         * void LocalFunction()
         * {
         *     Console.WriteLine("Local function called");
         * }
         * 
         * LocalFunction();
         * 
         * // Return exit code (optional)
         * return 0;
         */

        // ==============================================================================
        // SUPPORTING METHODS
        // ==============================================================================

        /// <summary>
        /// Demonstrates the main application logic
        /// This would contain your core business logic
        /// </summary>
        private static void ProcessMainLogic()
        {
            Console.WriteLine("\n--- Processing Main Application Logic ---");
            
            // Example: Calculate and display system information
            var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            Console.WriteLine($"Process ID: {currentProcess.Id}");
            Console.WriteLine($"Working Set Memory: {currentProcess.WorkingSet64 / 1024 / 1024} MB");
            Console.WriteLine($"Current Directory: {Environment.CurrentDirectory}");
            Console.WriteLine($"OS Version: {Environment.OSVersion}");
            Console.WriteLine($".NET Version: {Environment.Version}");
            
            // Simulate some work
            System.Threading.Thread.Sleep(1000); // Simulate processing time
        }

        /// <summary>
        /// Async initialization method - demonstrates async patterns in applications
        /// </summary>
        /// <returns>Task representing the async operation</returns>
        private static async Task InitializeApplicationAsync()
        {
            // Simulate async initialization work (e.g., loading config files, connecting to databases)
            Console.WriteLine("Loading configuration...");
            await Task.Delay(500); // Simulate async I/O operation
            
            Console.WriteLine("Establishing connections...");
            await Task.Delay(300); // Simulate connection setup
            
            Console.WriteLine("Initialization complete");
        }

        /// <summary>
        /// Process command line arguments asynchronously
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>Task representing the async operation</returns>
        private static async Task ProcessArgumentsAsync(string[] args)
        {
            Console.WriteLine($"\n--- Processing {args.Length} Arguments Async ---");
            
            foreach (var arg in args)
            {
                // Simulate async processing of each argument
                Console.WriteLine($"Processing argument: {arg}");
                await Task.Delay(100); // Simulate async work per argument
            }
        }

        /// <summary>
        /// Demonstrates async work that might be done in an application
        /// </summary>
        /// <returns>Task representing the async operation</returns>
        private static async Task PerformAsyncWork()
        {
            Console.WriteLine("\n--- Performing Async Work ---");
            
            // Simulate multiple concurrent async operations
            var tasks = new[]
            {
                SimulateNetworkCallAsync("API Service 1"),
                SimulateNetworkCallAsync("API Service 2"),
                SimulateFileOperationAsync("config.json")
            };
            
            // Wait for all async operations to complete
            await Task.WhenAll(tasks);
            
            Console.WriteLine("All async work completed");
        }

        /// <summary>
        /// Simulates an async network call
        /// </summary>
        /// <param name="serviceName">Name of the service being called</param>
        /// <returns>Task representing the async operation</returns>
        private static async Task SimulateNetworkCallAsync(string serviceName)
        {
            Console.WriteLine($"Calling {serviceName}...");
            await Task.Delay(800); // Simulate network latency
            Console.WriteLine($"Response received from {serviceName}");
        }

        /// <summary>
        /// Simulates an async file operation
        /// </summary>
        /// <param name="fileName">Name of the file being processed</param>
        /// <returns>Task representing the async operation</returns>
        private static async Task SimulateFileOperationAsync(string fileName)
        {
            Console.WriteLine($"Reading file: {fileName}...");
            await Task.Delay(400); // Simulate file I/O
            Console.WriteLine($"File {fileName} processed successfully");
        }
    }
}

// ==============================================================================
// ADDITIONAL ENTRY POINT PATTERNS AND NOTES
// ==============================================================================

/*
 * ENTRY POINT VARIATIONS AND BEST PRACTICES:
 * 
 * 1. VOID MAIN (No return value):
 *    static void Main(string[] args) { ... }
 *    - Use when you don't need to return an exit code
 *    - The runtime will return 0 by default
 * 
 * 2. INT MAIN (With return value):
 *    static int Main(string[] args) { ... }
 *    - Use when you need to return specific exit codes
 *    - 0 typically means success, non-zero means error
 * 
 * 3. ASYNC VOID MAIN:
 *    static async Task Main(string[] args) { ... }
 *    - Use for async applications without exit codes
 * 
 * 4. ASYNC INT MAIN:
 *    static async Task<int> Main(string[] args) { ... }
 *    - Use for async applications with exit codes
 * 
 * 5. PARAMETERLESS MAIN:
 *    static void Main() { ... }
 *    - Use when you don't need command line arguments
 * 
 * COMMAND LINE ARGUMENTS:
 * - args[0] is the first argument (NOT the program name in .NET)
 * - args.Length gives you the count of arguments
 * - Arguments are always strings; parse them as needed
 * 
 * EXIT CODES:
 * - 0: Success
 * - 1: General error
 * - 2: Misuse of shell command
 * - 126: Command invoked cannot execute
 * - 127: Command not found
 * - 128+n: Fatal error signal "n"
 * 
 * BEST PRACTICES:
 * - Use async Main for I/O-bound applications
 * - Always handle exceptions in Main
 * - Validate command line arguments early
 * - Use meaningful exit codes
 * - Consider using a command-line parsing library for complex arguments
 * - Log startup and shutdown events
 * - Initialize resources early, dispose them properly
 */


// paulreizjr: what if a .net project does not have a main method neither a program.cs file?
// 
// GitHub Copilot: Great question! If a .NET project doesn't have a `Main` method or Program.cs file, the behavior depends on the **project type**. Here are the different scenarios:
// 
// ## **1. Library Projects (.dll)**
// 
// ````csharp
// // Example: ClassLibrary.csproj
// <Project Sdk="Microsoft.NET.Sdk">
//   <PropertyGroup>
//     <TargetFramework>net8.0</TargetFramework>
//     <!-- No OutputType specified = Library by default -->
//   </PropertyGroup>
// </Project>
// 
// // Example library class
// public class MathUtilities
// {
//     /// <summary>
//     /// Library classes don't need entry points - they're consumed by other applications
//     /// </summary>
//     public static int Add(int a, int b) => a + b;
// }
// ````
// 
// **Result**: No entry point needed - libraries are consumed by other applications.
// 
// ## **2. Web Applications (ASP.NET Core)**
// 
// ````csharp
// // Minimal hosting model - implicit entry point
// var builder = WebApplication.CreateBuilder(args);
// 
// // Add services to the container
// builder.Services.AddControllers();
// 
// var app = builder.Build();
// 
// // Configure the HTTP request pipeline
// app.UseHttpsRedirection();
// app.MapControllers();
// 
// app.Run(); // This becomes the entry point
// ````
// 
// **Result**: The web framework provides the entry point through `app.Run()`.
// 
// ## **3. Projects with Global Using and Top-Level Programs**
// 
// ````csharp
// global using System;
// global using System.Threading.Tasks;
// 
// // filepath: Program.cs (C# 10+ minimal)
// // No explicit Main method - top-level statements become the entry point
// Console.WriteLine("This code runs as the entry point");
// 
// // Access to implicit 'args' parameter
// foreach (var arg in args)
// {
//     Console.WriteLine($"Arg: {arg}");
// }
// ````
// 
// **Result**: Compiler generates a `Main` method automatically from top-level statements.
// 
// ## **4. What Happens During Compilation**
// 
// ````csharp
// // What the compiler generates for top-level programs:
// // Original top-level code:
// Console.WriteLine("Hello World!");
// 
// // Compiler-generated equivalent:
// class Program
// {
//     static void Main(string[] args)
//     {
//         Console.WriteLine("Hello World!");
//     }
// }
// ````
// 
// ## **5. Build Errors Without Entry Points**
// 
// If you have an **executable project** without any entry point:
// 
// ````xml
// <!-- Project file indicating executable -->
// <Project Sdk="Microsoft.NET.Sdk">
//   <PropertyGroup>
//     <OutputType>Exe</OutputType>
//     <TargetFramework>net8.0</TargetFramework>
//   </PropertyGroup>
// </Project>
// ````
// 
// **Error**: `CS5001: Program does not contain a static 'Main' method suitable for an entry point`
// 
// ## **6. Multiple Entry Points Error**
// 
// ````csharp
// // This will cause a compilation error:
// class Program1
// {
//     static void Main() => Console.WriteLine("Entry 1");
// }
// 
// class Program2  
// {
//     static void Main() => Console.WriteLine("Entry 2");
// }
// ````
// 
// **Error**: `CS0017: Program has more than one entry point defined`
// 
// **Solution**: Use `/main` compiler option or `<StartupObject>` in project file:
// 
// ````xml
// <Project Sdk="Microsoft.NET.Sdk">
//   <PropertyGroup>
//     <OutputType>Exe</OutputType>
//     <TargetFramework>net8.0</TargetFramework>
//     <StartupObject>MyNamespace.Program1</StartupObject>
//   </PropertyGroup>
// </Project>
// ````
// 
// ## **7. Special Project Types**
// 
// ````csharp
// // Test Projects - No main method needed
// [TestClass]
// public class UnitTests
// {
//     [TestMethod]
//     public void TestMethod()
//     {
//         // Test runner provides the entry point
//         Assert.AreEqual(4, 2 + 2);
//     }
// }
// 
// // Worker Services - Framework provides entry point
// public class Program
// {
//     public static void Main(string[] args)
//     {
//         // Generated by template, but framework handles the heavy lifting
//         CreateHostBuilder(args).Build().Run();
//     }
//     
//     public static IHostBuilder CreateHostBuilder(string[] args) =>
//         Host.CreateDefaultBuilder(args)
//             .ConfigureServices((hostContext, services) =>
//             {
//                 services.AddHostedService<Worker>();
//             });
// }
// ````
// 
// ## **Summary**
// 
// - **Libraries**: Don't need entry points
// - **Web apps**: Framework provides entry point
// - **Console apps**: Must have `Main` or top-level statements  
// - **Test projects**: Test runner provides entry point
// - **Worker services**: Framework handles entry point
// 
// The compiler will generate appropriate entry points for top-level programs, but executable projects must have some form of entry point defined.