using System;

namespace Crash.ClientTestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Missing argument 'user'");
                return;
            
            }
            string userId = args[0];

            Console.WriteLine("Crash Test App");
            Console.WriteLine("\tc = Create Speck");
            Console.WriteLine("\tq = Quit");

            var client = new CrashClient(userId, new Uri("http://localhost:5141/Crash"));

            client.OnAdd += (user, speck) =>
            {
                Console.WriteLine($"Added! {speck.Id}");
            };

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