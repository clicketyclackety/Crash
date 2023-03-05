using System;

using Crash.Changes;
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
			Console.WriteLine("\tr = Relinquish Changes");
			Console.WriteLine("\td = Delete a Change");
			Console.WriteLine("\ts = Select a Change");
			Console.WriteLine("\tu = UnSelect a Change");
			Console.WriteLine("\tq = Quit");

			var crashDoc = new CrashDoc();
			var client = new CrashClient(crashDoc, userId, new Uri("http://localhost:5000/Crash"));

			client.OnAdd += (user, Change) =>
			{
				Console.WriteLine($"Added! {Change.Id}");
			};

			client.OnInitialize += Client_OnInitialize;
			client.StartAsync();

			while (true)
			{
				var k = Console.ReadKey();
				if (client.State != Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected)
				{
					Console.WriteLine(" --- Not Connected!");
					continue;
				}

				if (k.KeyChar == 'c')
				{
					client.AddAsync(new Change(Guid.NewGuid(), Environment.UserName, "Example payload"));
				}
				else if (k.KeyChar == 'r')
				{
					client.DoneAsync();
				}
				else if (k.KeyChar == 's')
				{
					client.SelectAsync(Guid.NewGuid());
				}
				else if (k.KeyChar == 'u')
				{
					client.UnselectAsync(Guid.NewGuid());
				}
				else if (k.KeyChar == 'd')
				{
					client.DeleteAsync(Guid.NewGuid());
				}
				else if (k.KeyChar == 'q')
				{
					break;
				}
			}

		}

		private static void Client_OnInitialize(Change[] obj)
		{
			;

		}
	}
}
