using System;

namespace ReadonlyKeywordExamples
{
	// readonly field: can be assigned only at declaration or in the constructor
	class ClassWithReadonlyField
	{
		public readonly int Id;
		public string Name;

		public ClassWithReadonlyField(int id, string name)
		{
			Id = id;
			Name = name;
		}
	}

	// readonly struct: a struct marked readonly; its instance members cannot mutate 'this'
	public readonly struct Point
	{
		public double X { get; }
		public double Y { get; }

		public Point(double x, double y) { X = x; Y = y; }

        // readonly method: guarantees it won't mutate instance state
        // Without readonly, when you call Deconstruct on a readonly struct variable (like ref readonly or in parameters), the compiler would create a defensive copy to prevent potential mutations. With the readonly modifier, no copy is needed since the compiler knows the method is safe.
        public double DistanceFromOrigin() => Math.Sqrt(X * X + Y * Y);

		// isso aqui da erro 
		// public double Teste()
		// {
		// 	X = X + Y;
		// 	return X;
        // }

		// readonly Deconstruct example
		public readonly void Deconstruct(out double x, out double y) { x = X; y = Y; }
	}

	class Program
	{
		// ref readonly return: returns a reference that cannot be used to modify the referred value
		static ref readonly Point GetPointRef(Point[] arr, int idx) => ref arr[idx];

		// 'in' parameter: pass by readonly reference (avoids a copy for large structs)
		static double ComputeDistance(in Point p) => p.DistanceFromOrigin();

		static void Main()
		{
			Console.WriteLine("1) readonly fields in classes:");
			var c = new ClassWithReadonlyField(42, "Alice");
			Console.WriteLine($"Id={c.Id}, Name={c.Name}");
			// c.Id = 99; // compile-time error: cannot assign to readonly field outside constructor

			Console.WriteLine();
			Console.WriteLine("2) readonly struct and readonly members:");
			var p = new Point(3, 4);
			Console.WriteLine($"Point: X={p.X}, Y={p.Y}, Distance={p.DistanceFromOrigin()}");
            // p.X = 0;
			p.Deconstruct(out double x, out double y);
			Console.WriteLine($"Deconstructed: x={x}, y={y}");
            p = new Point(5, 6);
            Console.WriteLine($"Point: X={p.X}, Y={p.Y}, Distance={p.DistanceFromOrigin()}");

            Console.WriteLine();
			Console.WriteLine("3) 'in' parameters (pass by readonly reference):");
			Console.WriteLine($"Distance via in parameter: {ComputeDistance(p)}");

			// Important: 'in' makes the reference readonly for the callee, but does not make
			// the referred object's internals immutable when the object is a reference type.
			var obj = new ClassWithReadonlyField(1, "Bob");
			MutateViaIn(in obj);
			Console.WriteLine($"After MutateViaIn: Name={obj.Name}");

			Console.WriteLine();
			Console.WriteLine("4) ref readonly return and local:");
			var arr = new Point[] { new Point(1, 2), new Point(10, 20) };
			ref readonly var r = ref GetPointRef(arr, 1);
			Console.WriteLine($"Ref readonly points to: X={r.X}, Y={r.Y}");
			// r = new Point(0,0); // compile-time error: cannot assign to ref readonly local

			Console.WriteLine();
			Console.WriteLine("Done.");
		}

		static void MutateViaIn(in ClassWithReadonlyField c)
		{
			// The 'in' modifier prevents reassigning the parameter reference inside the method,
			// but it does not prevent mutating the object that the reference points to.
			c.Name = c.Name + " (mutated)";
		}
	}
}
