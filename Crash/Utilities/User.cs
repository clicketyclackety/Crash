using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Drawing;

namespace Crash.Utilities
{

    public class User
    {
        public string name;
        public Color color;

        public static Color UserColor(string inputName)
        {
            var name = inputName;
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(name));
            Color color = Color.FromArgb(hash[0], hash[1], hash[2]);
            return color;
        }

        public static string CurrentUser = System.Environment.UserName;

    }

}
