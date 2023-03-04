using System.Collections;

using Crash.Common.Changes;
using Crash.Common.View;
using Crash.Geometry;


namespace Crash.Server.Tests
{

	[TestFixture]
	public class ArgHandlerTests
	{

		// [TestCaseSource(typeof(ArgHandlerData), nameof(ArgHandlerData.TestCases))]

		#region defaults

		[Test]
		public void EnsureDefaultURLs()
		{
			ArgumentHandler argHandler = new ArgumentHandler();
			argHandler.EnsureDefaults();

			Assert.That(Uri.TryCreate(argHandler.URL, UriKind.Absolute, out Uri? result), Is.True);
			Assert.That(result.AbsoluteUri.StartsWith(argHandler.URL), "URLs are not similiar enough", Is.True);
		}

		[Test]
		public void EnsureDefaultDbPath()
		{
			ArgumentHandler argHandler = new ArgumentHandler();
			argHandler.EnsureDefaults();
			string directory = Path.GetDirectoryName(argHandler.DatabaseFileName);

			Assert.That(argHandler.DatabaseFileName.EndsWith(".db"), "File does not end with .db", Is.True);
			Assert.That(Directory.Exists(directory), "Directory does not exist", Is.True);
			Assert.That(argHandler.DatabaseFileName.Length, Is.LessThan(255));
		}

		[Test]
		public void EnsureDefaultNewDb()
		{
			ArgumentHandler argHandler = new ArgumentHandler();
			argHandler.EnsureDefaults();

			Assert.That(argHandler.FreshDb, Is.False);
		}

		#endregion

		#region Argument Parsing


		#endregion

		#region URLs

		[TestCaseSource(typeof(ArgHandlerData), nameof(ArgHandlerData.URLArguments))]
		public bool ParseURLArgs(List<string> args)
		{
			ArgumentHandler argHandler = new ArgumentHandler();
			argHandler.EnsureDefaults();
			argHandler.ParseArgs(args.ToArray());

			ArgumentHandler defaultArgHandler = new ArgumentHandler();
			defaultArgHandler.EnsureDefaults();

			return argHandler.URL != defaultArgHandler.URL;
		}

		#endregion

		#region DBPaths

		[TestCaseSource(typeof(ArgHandlerData), nameof(ArgHandlerData.DBPathArguments))]
		public bool ParseDBArgs(List<string> args)
		{
			ArgumentHandler argHandler = new ArgumentHandler();
			argHandler.EnsureDefaults();
			argHandler.ParseArgs(args.ToArray());

			ArgumentHandler defaultArgHandler = new ArgumentHandler();
			defaultArgHandler.EnsureDefaults();

			return argHandler.DatabaseFileName != defaultArgHandler.DatabaseFileName;
		}

		#endregion

		#region ResetDbs

		#endregion

		#region Test Data

		public sealed class ArgHandlerData
		{
			public static IEnumerable TestCases
			{
				get
				{
					Camera camera1 = new Camera(CPoint.Origin, new CPoint(1, 2, 3));
					yield return CameraChange.CreateNew(camera1, "Jenny");
				}
			}

			public static IEnumerable URLArguments
			{
				get
				{
					// Trues

					for (int i = 0; i < 10; i++)
					{
						yield return new TestCaseData(new List<string> {
							"--urls", GetRandomValidFullURL(),
						}).Returns(true);
					}

					for (int i = 0; i < 10; i++)
					{
						yield return new TestCaseData(new List<string> {
							"--urls", GetRandomValidFullIpAddress(),
						}).Returns(true);
					}


					// Falses
					yield return new TestCaseData(new List<string> {
							"--rls", GetRandomValidFullURL(),
						}).Returns(false);

					yield return new TestCaseData(new List<string> {
							GetRandomValidFullURL(),
						}).Returns(false);

					/* Should this fail?
					yield return new TestCaseData(new List<string> {
							"--urls", "error", GetRandomValidFullURL(),
						}).Returns(false);
					*/

					yield return new TestCaseData(new List<string> {
							"URLS", GetRandomValidFullURL(),
						}).Returns(false);
				}
			}

			public static IEnumerable DBPathArguments
			{
				get
				{
					// Trues
					for (int i = 0; i < 5; i++)
					{
						yield return new TestCaseData(new List<string> {
							"--path", GetRandomValidDbFileName(),
						}).Returns(true);
					}

					/* Just FIlenames don't work currently
					yield return new TestCaseData(new List<string> {
							"--path", "fileName.db",
						}).Returns(true);
					*/

					yield return new TestCaseData(new List<string> {
							"--path", @"\App_Data\fileName.db",
						}).Returns(true);

					// Falses
					yield return new TestCaseData(new List<string> {
							"--pth", GetRandomValidDbFileName(),
						}).Returns(false);

					yield return new TestCaseData(new List<string> {
							"--pth", GetRandomValidDbFileName(),
						}).Returns(false);

					yield return new TestCaseData(new List<string> {
							GetRandomValidDbFileName(),
						}).Returns(false);

					yield return new TestCaseData(new List<string> {
							"--urls", GetRandomValidDbFileName(),
						}).Returns(false);

					yield return new TestCaseData(new List<string> {
							"--urls", "error", GetRandomValidDbFileName(),
						}).Returns(false);

					yield return new TestCaseData(new List<string> {
							"PATH", GetRandomValidDbFileName(),
						}).Returns(false);
				}
			}

			private static string GetRandomValidDbFileName()
			{
				string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
				string fileNameWithDbExt = $"{fileName}.db";
				string path = Directory.GetCurrentDirectory();

				return Path.Combine(path, fileName);
			}

			static string[] validPrefixes = new string[] { "http://", "https://", "" };

			private static string GetRandomValidFullURL()
			{
				int prefixIndex = TestContext.CurrentContext.Random.Next(0, validPrefixes.Length - 1);

				string prefix = validPrefixes[prefixIndex];
				string url = getRandomValidURL();
				string port = GetRandomValidPort();

				return $"{prefix}{url}{UrlPortSeparator}{port}";
			}

			private static string GetRandomValidFullIpAddress()
			{
				int prefixIndex = TestContext.CurrentContext.Random.Next(0, validPrefixes.Length - 1);

				string prefix = validPrefixes[prefixIndex];
				string ipAddress = getRandomValidIpAddress();
				string port = GetRandomValidPort();

				return $"{prefix}{ipAddress}{UrlPortSeparator}{port}";
			}

			const int portMin = 1000;
			const int intPortMax = 9000;
			private static string GetRandomValidPort()
				=> TestContext.CurrentContext.Random.Next(portMin, intPortMax).ToString();

			private static string getRandomValidURL()
			{
				char[] alphabet = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
				string[] domainExtensions = new string[] { ".com", ".au", ".ca", ".co.uk", ".org" };
				int urlCount = TestContext.CurrentContext.Random.Next(5, 20);

				string url = string.Empty;
				for (int i = 0; i < urlCount; i++)
				{
					int alphaCharIndex = TestContext.CurrentContext.Random.Next(0, alphabet.Length);
					url += alphabet[alphaCharIndex];
				}

				return url;
			}

			const int maxIPNumber = 255;
			const int ipNumberCount = 4;
			const char separator = '.';
			const char UrlPortSeparator = ':';
			private static string getRandomValidIpAddress()
			{
				string ipAddress = "";
				for (int i = 0; i < ipNumberCount; i++)
				{
					int ipNum = TestContext.CurrentContext.Random.Next(0, maxIPNumber);
					ipAddress += ipNum.ToString();
					if (i < ipNumberCount)
					{
						ipAddress += separator;
					}
				}

				return ipAddress;
			}

			#endregion

		}

	}

}
