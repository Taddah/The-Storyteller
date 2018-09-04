namespace The_Storyteller
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var b = new TheStoryteller())
            {
                b.RunAsync().Wait();
            }
        }
    }
}