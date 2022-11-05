using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Security.Cryptography;


namespace Crash.Utilities
{

    public class User
    {
        public string name;
        public Color color;

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

        public static string CurrentUser = System.Environment.UserName;

    // public static User CurrentUser
    // {
    //     get
    //     {
    //         return "";
    //     }
    //     set
    //     {
    //         name = value;
    //     }
    // }

    }

}
