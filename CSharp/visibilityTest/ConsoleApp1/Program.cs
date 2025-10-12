using test;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            VisibilitTestFactory visibilitTestFactory = new VisibilitTestFactory();

            IVisibilitTest test = visibilitTestFactory.GetTestClass() as IVisibilitTest;

            string x = "{\"data\": \"data text\"}";

            Console.WriteLine(x);

            Console.WriteLine(test.ReturnTest());
        }
    }
}
