using System.Collections.Generic;
using System.Linq;
using static System.Net.WebRequestMethods;

namespace Crash.Server.Tests
{

    [TestClass]
    public class ServerEntryPoint
    {
        public static IEnumerable<object[]> GetValidURLArgs()
        {
             return new List<string[]>
             {
                new string[] { "--urls", "https://thisisvalidaurl.com" },
                new string[] { "--URLs", "www.thisisavalidurl.com" },
                new string[] { "--urLs", "127.0.0.1:8080" },
                new string[] { "--uRls", "127.0.0.1:80" },
                new string[] { "--uRLs", "127.0.0.1:5000" },
                new string[] { "--urLs", "127.0.0.1:5000/" },
                new string[] { "--URLs", "https://thisisvalidaurl.com" },
                new string[] { "--URLs", "https://thisisvalidaurl.com//" },
             };
        }

        public static IEnumerable<object[]> GetInvalidURLArgs()
        {
            return new List<string[]>
            {
                new string[] { "urls", "https://thisisavalidurl.com"} ,         // No dashes
                new string[] { "--urls", "https://thisisaninvalidurl..com"} ,   // Invalid URL
                new string[] { null, "https://thisisavalidurl.com"} ,             // No preposition
                new string[] { "-urls", "https://thisisavalidurl.com"} ,        // One dash
                new string[] { "--urls", "127.0.0.1"} ,                         // Missing Port
                new string[] { "--urls", "127.0.0.1/"} ,                         // Missing Port with Slash
                // new string[] { "--urls", "127.0.0.1.3"} ,                       // Invalid IP // Detects as URL?
                // new string[] { "--urls", "999.888.777.666"} ,                   // Invalid IP // Detects as URL?
                new string[] { "--urls", "127.0.0.1:9999999"} ,                 // Invalid Port
            };
        }

        public static IEnumerable<object[]> GetValidPathArgs()
        {
            return new List<string[]>
            {
                new string[] { "--path", @"C:\MyPrograms\File" },
                new string[] { "--pAtH", @"C:\MyPrograms\File\" },
                new string[] { "--PATH", @"C:\MyPrograms\File\" },
                new string[] { "--PATH", @"C:\MyPrograms\File" },
                new string[] { "--PATH", @"C:\MyPrograms\Dir 2" },
                new string[] { "--PATH", @"C:/MyPrograms/Dir 2" },
                new string[] { "--PATH", @"C://MyPrograms//File//" },
                new string[] { "--PATH", "C:\\MyPrograms\\File\\" },
                new string[] { "--PATH", "MyPrograms\\File\\" },
                new string[] { "--PATH", "\\MyPrograms\\File\\" },
            };
        }

        public static IEnumerable<object[]> GetInvalidPathArgs()
        {
            return new List<string[]>
            {
                new string[] { "path", @"C:\MyPrograms\Data\"} ,          // Missing double dash
                new string[] { "-PATH", @"C:\MyPrograms\File"} ,       // One Dash
                new string[] { "--PATH", @"C:\MyPrograms\File.db"} ,      // Includes file extension
            };
        }

        private static IEnumerable<object[]> Zipper(IEnumerable<object[]> one, IEnumerable<object[]> two)
        {
            var oneEnumer = one.GetEnumerator();
            var twoEnumer = two.GetEnumerator();

            while (oneEnumer.MoveNext() & twoEnumer.MoveNext())
            {
                string[] oneArgs = oneEnumer.Current.Cast<string>().ToArray();
                string[] twoArgs = twoEnumer.Current.Cast<string>().ToArray();

                yield return new string[] { oneArgs[0], oneArgs[1], twoArgs[0], twoArgs[1] };
            }
        }

        public static IEnumerable<object[]> GetAllValidArgs()
            => Zipper(GetValidURLArgs(), GetValidPathArgs());

        public static IEnumerable<object[]> GetAllInvalidArgs()
            => Zipper(GetInvalidURLArgs(), GetInvalidPathArgs());


        [TestMethod]
        [DynamicData(nameof(GetValidURLArgs), DynamicDataSourceType.Method)]
        public void UrlArgs_Valid(string? urlInput, string? url)
        {
            string[] args = new string[2] { urlInput, url };

            var argHandler = new ArgumentHandler();
            argHandler.ParseArgs(args);

            Assert.IsFalse(string.IsNullOrEmpty(argHandler.URL), argHandler.URL);
        }

        [TestMethod]
        [DynamicData(nameof(GetInvalidURLArgs), DynamicDataSourceType.Method)]
        public void UrlArgs_Invalid(string? urlInput, string? url)
        {
            string[] args = new string[2] { urlInput, url };

            var argHandler = new ArgumentHandler();
            argHandler.ParseArgs(args);

            Assert.IsTrue(string.IsNullOrEmpty(argHandler.URL));
        }

        [TestMethod]
        [DynamicData(nameof(GetValidPathArgs), DynamicDataSourceType.Method)]
        public void PathArgs_Valid(string? pathInput, string? path)
        {
            string[] args = new string[2] { pathInput, path };

            var argHandler = new ArgumentHandler();
            argHandler.ParseArgs(args);

            Assert.IsFalse(string.IsNullOrEmpty(argHandler.DatabaseFileName),
                            argHandler.DatabaseFileName);
        }

        [TestMethod]
        [DynamicData(nameof(GetInvalidPathArgs), DynamicDataSourceType.Method)]
        public void PathArgs_InValid(string? pathInput, string? path)
        {
            string[] args = new string[2] { pathInput, path };

            var argHandler = new ArgumentHandler();
            argHandler.ParseArgs(args);

            Assert.IsTrue(string.IsNullOrEmpty(argHandler.DatabaseFileName));
        }

        [TestMethod]
        [DynamicData(nameof(GetAllValidArgs), DynamicDataSourceType.Method)]
        public void PathAndUrlArgs_Valid(string? urlInput, string? url, string? pathInput, string? path)
        {
            string[] args = new string[4] { urlInput, url, pathInput, path };
            var argHandler = new ArgumentHandler();
            argHandler.ParseArgs(args);

            Assert.IsFalse(string.IsNullOrEmpty(argHandler.DatabaseFileName));
            Assert.IsFalse(string.IsNullOrEmpty(argHandler.URL));
        }

        [TestMethod]
        [DynamicData(nameof(GetAllInvalidArgs), DynamicDataSourceType.Method)]
        public void PathAndUrlArgs_InValid(string? urlInput, string? url, string? pathInput, string? path)
        {
            string[] args = new string[4] { urlInput, url, pathInput, path };
            var argHandler = new ArgumentHandler();
            argHandler.ParseArgs(args);

            Assert.IsTrue(string.IsNullOrEmpty(argHandler.DatabaseFileName));
            Assert.IsTrue(string.IsNullOrEmpty(argHandler.URL));
        }

        [TestMethod]
        [DynamicData(nameof(GetAllInvalidArgs), DynamicDataSourceType.Method)]
        public void EnsureDefaults_IsValid(string? urlInput, string? url, string? pathInput, string? path)
        {
            string[] args = new string[4] { urlInput, url, pathInput, path };
            var argHandler = new ArgumentHandler();
            argHandler.ParseArgs(args);
            argHandler.EnsureDefaults();

            Assert.IsFalse(string.IsNullOrEmpty(argHandler.DatabaseFileName));
            Assert.IsFalse(string.IsNullOrEmpty(argHandler.URL));
        }

    }

}