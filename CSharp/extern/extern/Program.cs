using System;
using System.Runtime.InteropServices;

class Program
{
    // Example 1: P/Invoke - Calling an external function from a DLL
    [DllImport("kernel32.dll")]
    extern static uint GetTickCount();

    // Example 2: Extern alias (requires alias setup in .csproj)
    // extern alias MyAlias;
    // using MyAlias::System.Collections;

    static void Main()
    {
        // Using the extern P/Invoke method
        uint ticks = GetTickCount();
        Console.WriteLine($"Ticks since system start: {ticks}");

        // Example of using extern alias (commented out as it requires setup)
        // var list = new List<int>(); // This would use the aliased namespace if set up
    }
}
