using System.Diagnostics;
using System.Reflection;

// Example demonstrating how to track calling methods using reflection
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Reflection Call Stack Example ===\n");
        
        var classA = new ClassA();
        
        Console.WriteLine("Calling ClassA.Method1:");
        classA.Method1();
        
        Console.WriteLine("\nCalling ClassA.Method2:");
        classA.Method2();
    }
}

class ClassA
{
    private ClassB classB = new ClassB();
    
    public void Method1()
    {
        Console.WriteLine("ClassA.Method1 called");
        classB.Method1();
    }
    
    public void Method2()
    {
        Console.WriteLine("ClassA.Method2 called");
        classB.Method1();
    }
}

class ClassB
{
    private ClassC classC = new ClassC();
    
    public void Method1()
    {
        Console.WriteLine("ClassB.Method1 called");
        classC.Method1();
    }
}

class ClassC
{
    public void Method1()
    {
        Console.WriteLine("ClassC.Method1 called");
        
        // Method 1: Using StackTrace to get the calling method information
        Console.WriteLine("\n--- Using StackTrace ---");
        var stackTrace = new StackTrace();
        
        // Walk through the stack frames to find the ClassA method
        for (int i = 0; i < stackTrace.FrameCount; i++)
        {
            var frame = stackTrace.GetFrame(i);
            var method = frame?.GetMethod();
            
            if (method != null)
            {
                Console.WriteLine($"Frame {i}: {method.DeclaringType?.Name}.{method.Name}");
                
                // Check if this frame belongs to ClassA
                if (method.DeclaringType?.Name == "ClassA")
                {
                    Console.WriteLine($"*** Found ClassA method: {method.Name} ***");
                    break;
                }
            }
        }
        
        // Method 2: Using CallerMemberName attribute (alternative approach)
        Console.WriteLine("\n--- Alternative: Get immediate caller ---");
        GetImmediateCaller();
        
        // Method 3: Get specific caller by type
        Console.WriteLine("\n--- Get specific caller by type ---");
        var classAMethod = GetCallerFromType(typeof(ClassA));
        if (classAMethod != null)
        {
            Console.WriteLine($"ClassA method that initiated this call: {classAMethod.Name}");
        }
    }
    
    private void GetImmediateCaller()
    {
        var stackTrace = new StackTrace();
        // Frame 0 = current method (GetImmediateCaller)
        // Frame 1 = Method1
        // Frame 2 = ClassB.Method1
        // Frame 3 = ClassA.Method1 or ClassA.Method2
        
        var callerFrame = stackTrace.GetFrame(3);
        var callerMethod = callerFrame?.GetMethod();
        
        if (callerMethod != null)
        {
            Console.WriteLine($"Immediate ClassA caller: {callerMethod.DeclaringType?.Name}.{callerMethod.Name}");
        }
    }
    
    private MethodBase? GetCallerFromType(Type targetType)
    {
        var stackTrace = new StackTrace();
        
        for (int i = 0; i < stackTrace.FrameCount; i++)
        {
            var frame = stackTrace.GetFrame(i);
            var method = frame?.GetMethod();
            
            if (method?.DeclaringType == targetType)
            {
                return method;
            }
        }
        
        return null;
    }
}
