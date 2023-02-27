using System;

using Crash.Client;
using Crash.Common.Document;

namespace Crash.ClientTestApp
{
	/// <summary>
	/// Test program for server connection
	/// </summary>
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
			Console.WriteLine("\tc = Create Change");
			Console.WriteLine("\tq = Quit");

			var crashDoc = new CrashDoc();
			var client = new CrashClient(crashDoc, userId, new Uri("http://localhost:8080/Crash"));

			client.OnAdd += (user, Change) =>
			{
				Console.WriteLine($"Added! {Change.Id}");
			};

			client.StartAsync().RunSynchronously();

			while (true)
			{
				var k = Console.ReadKey();
				if (k.KeyChar == 'c')
				{
					client.AddAsync(Change.CreateEmpty())?.Wait();
				}
				if (k.KeyChar == 'q')
				{
					break;
				}
			}

		}
	}
}
