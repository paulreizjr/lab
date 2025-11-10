namespace ClassLibrary1
{
    public class StupidClass : IStupidInterface
    {
        public string? MyProperty { get; set; }
        public string Hi() => $"Hello from StupidClass: {MyProperty}";
    }
}
