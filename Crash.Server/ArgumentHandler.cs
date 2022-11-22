using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace Crash.Server
{
    public class ArgumentHandler
    {
        const char separator = '\n';
        const string pattern = @"--([\w]+ [\S]+)";
        const string dbName = "Database.db";
        const string appName = "Crash";
        const string dbDirectory = "App_Data";

        public string URL { get; private set; }

        public string DatabaseFileName { get; private set; }

        private Dictionary<string, Action<string>> argDict;

        public ArgumentHandler()
        {
            argDict = new Dictionary<string, Action<string>>
            {
                { "URL", _handleUrlArgs },
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

                if (argSplit?.Count() > 0)
                    argValue = argSplit[1];

                _handleArgs(argPreposition, argValue);
            }
        }

        public void EnsureDefaults()
        {
            if (string.IsNullOrEmpty(URL))
            {
                // ... 
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
                catch (ArgumentException aEx)
                {
                    Console.WriteLine(aEx.Message);
                }
            }
            else
            {
                Console.WriteLine("Invalid argument");
            }
        }

        #region URL Args

        private void _handleUrlArgs(string urlValue)
        {
            if (!_validateUrl(ref urlValue))
            {
                throw new ArgumentException("Given URL was Invalid.");
            }
            else
            {
                URL = urlValue;
            }
        }

        private bool _validateUrl(ref string url)
        {
            UriBuilder uriBuild;
            try
            {
                uriBuild = new UriBuilder(url);
            }
            catch(UriFormatException)
            {
                throw new ArgumentException("Invalid URL");
            }

            return true;
        }

        #endregion

        #region Databsae Args

        private void _handleDatabasePath(string givenPath)
        {
            if (!_validateDatabaseDirectory(givenPath)) return;
            _ensureDatabaseDirectoryExists(givenPath);
            _setDatabaseFilePath(givenPath);
        }

        private bool _validateDatabaseDirectory(string givenPath)
        {
            if (!Uri.IsWellFormedUriString(givenPath, UriKind.RelativeOrAbsolute)) return false;
            if (Path.GetInvalidPathChars().Where(c => givenPath.Contains(c)).Any()) return false;
            string fileName = Path.GetFileName(givenPath);
            if (!string.IsNullOrEmpty(fileName)) return false;

            return true;
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
