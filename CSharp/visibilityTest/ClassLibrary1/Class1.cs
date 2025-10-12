namespace test
{
    public interface IVisibilitTest
    {
        string ReturnTest();
    }

    internal class TestClass : IVisibilitTest
    {
        public string ReturnTest()
        {
            return "test";
        }
    }

    public class VisibilitTestFactory
    {
        public IVisibilitTest GetTestClass()
        {
            return new TestClass(); // will TestClass be accessable in another assembly? 
            // 
        }
    }
}