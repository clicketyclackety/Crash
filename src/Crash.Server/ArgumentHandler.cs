using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace Crash.Server
{
    public class ArgumentHandler
    {

        const string pattern = @"--([\w]+ [\S]+)";
        const string dbName = "Database.db";
        const string appName = "Crash";
        const string dbDirectory = "App_Data";
        const string defaultURL = "127.0.0.1:8080";

        public string URL { get; private set; }

        public string DatabaseFileName { get; private set; }

        private readonly Dictionary<string, Action<string>> argDict;

        public ArgumentHandler()
        {
            argDict = new Dictionary<string, Action<string>>
            {
                { "URLS", _handleUrlArgs },
                { "PATH", _handleDatabasePath },
                { "HELP", _handleHelpRequest }
            };
        }

        public void ParseArgs(string[] args)
        {
            string flatArgs = string.Join(' ', args);
            MatchCollection argMatches = Regex.Matches(flatArgs, pattern, RegexOptions.IgnoreCase);

            foreach(Match argMatch in argMatches)
            {
                Group? group = argMatch?.Groups.Values?.LastOrDefault();
                if (null == group) continue;

                string[] argSplit = group.Value.Split(' ', 2);

                if (argSplit?.Count() == 0) continue;

                string argPreposition = argSplit[0];
                string argValue = string.Empty;

                if (argSplit?.Length > 0)
                    argValue = argSplit[1];

                _handleArgs(argPreposition, argValue);
            }
        }

        public void EnsureDefaults()
        {
            if (string.IsNullOrEmpty(URL))
            {
                _setUrl(defaultURL);
            }

            if (string.IsNullOrEmpty(DatabaseFileName))
            {
                string databasePath = _getDefaultDatabaseDirectory();
                _handleDatabasePath(databasePath);
                _setDatabaseFilePath(databasePath);
            }
        }

        private void _handleArgs(string argPreposition, string argValue)
        {
            if (argDict.TryGetValue(argPreposition.ToUpper(), out Action<string> @action))
            {
                try
                {
                    action.Invoke(argValue);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine($"Invalid argument {argPreposition} and value {argValue}");
            }
        }

        #region URL Args

        private void _handleUrlArgs(string urlValue)
        {
            _validateUrlInput(ref urlValue);
            if (!_validateUrlInput(ref urlValue))
            {
                throw new ArgumentException("Given URL was Invalid.");
            }
            else
            {
                _setUrl(urlValue);
            }
        }

        private void _setUrl(string urlValue)
        {
            URL = urlValue;
        }

        private bool _validateUrlInput(ref string url)
        {
            UriBuilder uriBuild;
            try
            {
                uriBuild = new UriBuilder(url);
                if (uriBuild.Uri.HostNameType is UriHostNameType.IPv4 or UriHostNameType.IPv6)
                {
                    return _validateIPAddress(url);
                }
                else if (uriBuild.Uri.HostNameType is UriHostNameType.Dns)
                {
                    return _validateUrl(url);
                }

                throw new ArgumentException("Invalid URL. Was not detectable as either an IP or URL");
            }
            catch(UriFormatException)
            {
                throw new ArgumentException("Invalid URL");
            }
        }

        private bool _validateUrl(string url)
        {
            // No logic required for now.
            return true;
        }
        
        private bool _validateIPAddress(string url)
        {
            UriBuilder uriBuild = new UriBuilder(url);

            if (!uriBuild.Uri.IsDefaultPort) return true;
            if (url.Replace("/", "").EndsWith(uriBuild.Port.ToString())) return true;

            string uriii = uriBuild.ToString();

            // IP is IPv4/IPv6 - Website is DNS
            if (uriBuild.Uri.HostNameType is UriHostNameType.IPv4 or UriHostNameType.IPv6)
            {
                throw new ArgumentException("Port is required for IP Address");
            }

            return true;
        }

        #endregion

        #region Databsae Args

        private void _handleDatabasePath(string givenPath)
        {
            _validateDatabaseDirectory(givenPath);
            _ensureDatabaseDirectoryExists(givenPath);
            _setDatabaseFilePath(givenPath);
        }

        private void _validateDatabaseDirectory(string givenPath)
        {
            DirectoryInfo dInfo = new DirectoryInfo(givenPath);
            string wellFormattedPath = dInfo.FullName;
            
            if (!string.IsNullOrEmpty(dInfo.Extension))
            {
                throw new Exception("Do not feed in a file Name!");
            }

            // if (!Uri.IsWellFormedUriString(dInfo.FullName, UriKind.RelativeOrAbsolute)) return false;
            if (Path.GetInvalidPathChars().Where(c => wellFormattedPath.Contains(c)).Any())
            {
                throw new Exception("Invalid Characters in given path!");
            }
        }

        private string _getDefaultDatabaseDirectory()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string databasePath = Path.Combine(appData, appName, dbDirectory);
            string databaseDirectory = Path.Combine(databasePath);

            return databaseDirectory;
        }

        private void _setDatabaseFilePath(string databasePath)
        {
            // databasePath.Replace("\\", "/");
            DatabaseFileName = Path.Combine(databasePath, dbName);
        }

        private void _ensureDatabaseDirectoryExists(string databaseFilePath)
        {
            if (!Directory.Exists(databaseFilePath))
            {
                Directory.CreateDirectory(databaseFilePath);
            }
        }

        #endregion

        #region Help
        private void _handleHelpRequest(string helpArg)
        {
            // ... print help
            string supportedMessage = $"There are 3 supported commands : {string.Join(", ", argDict)} ";
            Console.WriteLine(supportedMessage);

            string syntaxMessages = "Commands must be prefixed with --";
            Console.WriteLine(syntaxMessages);

        }

        #endregion

    }

}
