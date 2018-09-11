namespace The_Storyteller
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Start the bot ! YAY !
            using (var b = new TheStoryteller())
            {
                b.RunAsync().Wait();
            }
        }
    }
}