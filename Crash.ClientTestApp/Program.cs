using System;

namespace Crash.ClientTestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Crash Test App");
            Console.WriteLine("\tc = Create Speck");
            Console.WriteLine("\tq = Quit");

            var client = new CrashClient("user1", new Uri("http://localhost:5141/Crash"));

            client.StartAsync().Wait();

            while (true)
            {
                var k = Console.ReadKey();
                if (k.KeyChar == 'c')
                {
                    client.Add(new SpeckLib.Speck())?.Wait();
                }
                if (k.KeyChar == 'q')
                {
                    break;
                }
            }

        }
    }
}