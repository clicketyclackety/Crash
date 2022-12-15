using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Security.Cryptography;


namespace Crash.Utilities
{
    /// <summary>
    /// User class
    /// </summary>
    public sealed class User
    {
        /// <summary>
        /// Name of the user
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Color of the user
        /// </summary>
        public Color color { get; set; }

        /// <summary>
        /// User Constructor 
        /// </summary>
        /// <param name="inputName">the name of the user</param>
        public User(string inputName)
        {
            name = inputName;

            if (string.IsNullOrEmpty(inputName))
            {
                color = Color.Gray;
            }
            else
            {
                var md5 = MD5.Create();
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(name));
                color = Color.FromArgb(hash[0], hash[1], hash[2]);
            }
        }

        /// <summary>
        /// Current user
        /// </summary>
        public static User CurrentUser {get; set;}

    }

}
