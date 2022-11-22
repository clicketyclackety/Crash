namespace Crash.Server.Tests
{

    [TestClass]
    public class ServerEntryPoint
    {

        [TestMethod]
        [DataRow("--url https://thisisvalidaurl.com")]
        [DataRow("--url www.thisisavalidurl.com")]
        [DataRow("--url 127.0.0.1:8080")]
        [DataRow("--URL https://thisisvalidaurl.com")]
        [DataRow("--uRl https://thisisvalidaurl.com")]
        public void UrlArgs_Valid(string argsIn)
        {
            string[] args = argsIn.Split(' ', 2);

            var argHandler = new ArgumentHandler();
            argHandler.ParseArgs(args);

            Assert.IsFalse(string.IsNullOrEmpty(argHandler.URL), argHandler.URL);
        }

        [TestMethod]
        [DataRow("url https://thisisavalidurl.com")]        // No dashes
        [DataRow("--url https://thisisaninvalidurl..com")]  // Invalid URL
        [DataRow("https://thisisavalidurl.com")]            // No preposition
        [DataRow("-url https://thisisavalidurl.com")]       // One dash
        [DataRow("--url 127.0.0.1")]                        // Missing Port
        [DataRow("--url 127.0.0.1.3")]                      // Invalid IP
        [DataRow("--url 999.888.777.666")]                  // Invalid IP
        public void UrlArgs_Invalid(string argsIn)
        {
            string[] args = argsIn.Split(' ', 2);

            var argHandler = new ArgumentHandler();
            argHandler.ParseArgs(args);

            Assert.IsTrue(string.IsNullOrEmpty(argHandler.URL));
        }

        [TestMethod]
        [DataRow(@"--path C:\MyPrograms\File")]
        [DataRow(@"--pAtH C:\MyPrograms\File\")]
        [DataRow(@"--PATH C:\MyPrograms\File\")]
        [DataRow(@"--PATH C:\MyPrograms\File")]
        [DataRow(@"--PATH C:\MyPrograms\Dir 2")]
        [DataRow(@"--PATH C:/MyPrograms/Dir 2")]
        [DataRow(@"--PATH C://MyPrograms//File//")]
        [DataRow("--PATH C:\\MyPrograms\\File\\")]
        public void PathArgs_Valid(string argsIn)
        {
            string[] args = argsIn.Split(' ', 2);

            var argHandler = new ArgumentHandler();
            argHandler.ParseArgs(args);

            Assert.IsFalse(string.IsNullOrEmpty(argHandler.DatabaseFileName),
                            argHandler.DatabaseFileName);
        }

        [TestMethod]
        [DataRow(@"path C:\MyPrograms\Data\")]          // Missing double dash
        [DataRow(@"-PATH C:\MyPrograms\File.db")]       // One Dash
        [DataRow(@"--pAtH C:\MyPrograms\File")]         // Missing file extension
        [DataRow(@"--PATH C:\MyPrograms\db")]           // Missing
        [DataRow(@"--PATH \MyPrograms\File\\\")]        // Invalid Path
        [DataRow(@"--PATH C:\MyPrograms\File.db")]      // Includes file extension
        [DataRow(@"--PATH C:\MyPrograms\MyPrograms\MyPrograms\MyPrograms\"+
            @"MyPrograms\MyPrograms\MyPrograms\MyPrograms\MyPrograms\MyPrograms\"+
            @"MyPrograms\MyPrograms\MyPrograms\MyPrograms\MyPrograms\MyPrograms\" +
            @"MyPrograms\MyPrograms\MyPrograms\MyPrograms\MyPrograms\MyPrograms\" +
            @"MyPrograms\MyPrograms\MyPrograms\MyPrograms\MyPrograms\MyPrograms\" +
            @"MyPrograms\MyPrograms\MyPrograms\MyPrograms\MyPrograms\MyPrograms\")]      // Too Long
        public void PathArgs_InValid(string argsIn)
        {
            string[] args = argsIn.Split(' ', 2);

            var argHandler = new ArgumentHandler();
            argHandler.ParseArgs(args);

            Assert.IsTrue(string.IsNullOrEmpty(argHandler.DatabaseFileName));
        }

        [TestMethod]
        [DataRow(" ")]
        [DataRow(" ")]
        [DataRow(" ")]
        [DataRow(" ")]
        [DataRow(" ")]
        public void PathAndUrlArgs_Valid(string argsIn)
        {

        }

        [TestMethod]
        [DataRow(" ")]
        [DataRow(" ")]
        [DataRow(" ")]
        [DataRow(" ")]
        [DataRow(" ")]
        public void PathAndUrlArgs_InValid(string argsIn)
        {

        }

    }

}