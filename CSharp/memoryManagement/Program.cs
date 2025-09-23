using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

// Small demo: managed vs native allocation
// - Managed: allocations go on the CLR heap and are tracked by the GC
// - stackalloc: short-lived allocation on the stack (no GC bookkeeping)
// - Native (AllocHGlobal): unmanaged memory allocated from the OS; must be freed
// - Pinning: prevents the GC from moving an object so native code can take its address

// quando falamos de unmanaged allocation, estamos falando de alocação fora do heap gerenciado
// ou seja, a memória é alocada diretamente do sistema operacional, sem o controle do garbage collector (GC)
// podemos dizer que esta memória é a heap? 
// tecnicamente, a heap gerenciada é uma região de memória controlada pelo CLR e o GC
// já a memória alocada via Marshal.AllocHGlobal, por exemplo, é uma região separada, não gerenciada pelo GC
// portanto, embora ambas sejam "heaps" no sentido tradicional (áreas de memória para alocação dinâmica),
// no contexto do .NET, chamamos a memória alocada via AllocHGlobal de "unmanaged memory" ou "native heap"
// para evitar confusão, é melhor referir-se a ela como "unmanaged memory"
// A alocação nativa é necessária quando precisamos interagir com código não gerenciado (nativo),
// ou quando precisamos de controle mais fino sobre o ciclo de vida da memória
// isso é útil para interoperabilidade com código nativo (código do próprio sistema operacional), ou para otimizar performance em cenários específicos
// mas requer cuidado extra para evitar vazamentos de memória e outros problemas

// why native allocation has better performance in some scenarios?
// Native allocation can be faster in scenarios where you need large blocks of memory that persist for a long time
// because it avoids the overhead of the GC's bookkeeping and potential pauses

// pinning é o processo de fixar um objeto na memória para evitar que o GC o mova
// isso é necessário quando se passa um ponteiro para código nativo, garantindo que o endereço permaneça válido
// porém, o uso excessivo de pinning pode fragmentar o heap gerenciado e impactar a performance do GC

// when we create an object in C# that implements IDisposable, we should always ensure Dispose is called
// this can be done via a 'using' statement or a try/finally block
// where an object that implements IDisposable is allocated? in the managed heap?
// yes, all managed objects are allocated on the managed heap
// however, the unmanaged resources they wrap (like file handles, native memory) are outside the managed heap
// the Dispose method is responsible for releasing those unmanaged resources
// does the GC keeps track of objects that implement IDisposable?
// the GC tracks all managed objects, regardless of whether they implement IDisposable

class Program
{
    static void Main()
    {
        Console.WriteLine("Managed vs Native allocation demo\n");

        ManagedAllocationDemo();
        Console.WriteLine();

        StackAllocDemo();
        Console.WriteLine();

        UnmanagedAllocationDemo();
        Console.WriteLine();

        PinningDemo();
        Console.WriteLine();

        Console.WriteLine("Demo complete.");
    }

    static void ManagedAllocationDemo()
    {
        Console.WriteLine("1) Managed allocations (CLR heap)");
        long before = GC.GetTotalMemory(false);
        Console.WriteLine($"  GC.GetTotalMemory(before) = {before:N0} bytes");

        // Allocate many small arrays to create pressure on the managed heap (~10 MB)
        var list = new List<byte[]>();
        for (int i = 0; i < 10_000; i++)
        {
            list.Add(new byte[1024]); // 1 KiB each
        }

        long afterAlloc = GC.GetTotalMemory(false);
        Console.WriteLine($"  Allocated ~10k * 1KiB -> GC.GetTotalMemory(after) = {afterAlloc:N0} bytes (delta {afterAlloc - before:N0})");

        // Release references and force a full GC
        list = null;
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long afterCollect = GC.GetTotalMemory(true);
        Console.WriteLine($"  After GC.GetTotalMemory(true) = {afterCollect:N0} bytes (should be close to before)");
    }

    static void StackAllocDemo()
    {
        Console.WriteLine("2) stackalloc (stack allocation, no GC bookkeeping)");
        long before = GC.GetTotalMemory(false);

        // stackalloc allocates memory on the current thread's stack. Lifetime is limited to the method/scope.
        Span<byte> buffer = stackalloc byte[1024];
        for (int i = 0; i < buffer.Length; i++) buffer[i] = (byte)(i & 0xFF);

        long after = GC.GetTotalMemory(false);
        Console.WriteLine($"  GC.GetTotalMemory(before) = {before:N0}, after stackalloc = {after:N0} (unchanged)");

        // Use buffer to avoid it being optimized away
        Console.WriteLine($"  buffer[0]={buffer[0]}, buffer[512]={buffer[512]}");
    }

    static void UnmanagedAllocationDemo()
    {
        Console.WriteLine("3) Unmanaged allocation via Marshal.AllocHGlobal");
        long managedBefore = GC.GetTotalMemory(false);
        Console.WriteLine($"  Managed heap before = {managedBefore:N0} bytes");

        const int bytes = 8 * 1024 * 1024; // 8 MiB
        IntPtr ptr = IntPtr.Zero;
        try
        {
            ptr = Marshal.AllocHGlobal(bytes);
            Console.WriteLine($"  Allocated {bytes:N0} bytes unmanaged at {ptr}");

            // Fill the unmanaged memory with a pattern using a managed buffer and Marshal.Copy
            var fill = new byte[4096];
            for (int i = 0; i < fill.Length; i++) fill[i] = (byte)(i & 0xFF);

            int remaining = bytes;
            long offset = 0;
            while (remaining > 0)
            {
                int chunk = Math.Min(remaining, fill.Length);
                Marshal.Copy(fill, 0, ptr + (int)offset, chunk);
                remaining -= chunk;
                offset += chunk;
            }

            Console.WriteLine("  Wrote pattern into unmanaged memory (via Marshal.Copy)");

            long managedAfter = GC.GetTotalMemory(false);
            Console.WriteLine($"  Managed heap after unmanaged write = {managedAfter:N0} bytes (unchanged apart from small buffers)");
        }
        finally
        {
            if (ptr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(ptr);
                Console.WriteLine("  Freed unmanaged memory (Marshal.FreeHGlobal)");
            }
        }
    }

    static void PinningDemo()
    {
        Console.WriteLine("4) Pinning a managed object to obtain a stable address");

        var arr = new byte[1_000_000];
        for (int i = 0; i < arr.Length; i += 4096) arr[i] = 0xAA; // touch some pages

        GC.Collect(); // try to move things before pinning

        GCHandle handle = default;
        try
        {
            handle = GCHandle.Alloc(arr, GCHandleType.Pinned);
            IntPtr addr = handle.AddrOfPinnedObject();
            Console.WriteLine($"  Pinned managed array at address: {addr}");

            // When pinned, the GC won't move 'arr'. Overusing pinning fragments the GC heap.
        }
        finally
        {
            if (handle.IsAllocated) handle.Free();
            Console.WriteLine("  Unpinned managed array");
        }
    }
}
