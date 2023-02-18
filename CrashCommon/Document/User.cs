using System.Security.Cryptography;
using System.Drawing;
using System.Text;


namespace Crash.Utilities
{

    public enum CameraState
    {
        None = 0,
        Visible = 1,
        Follow = 2,
    }

    /// <summary>
    /// User class
    /// </summary>
    public sealed class User
    {

        public bool Visible { get; set; } = true;

        /// <summary>
        /// Name of the user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Color of the user
        /// </summary>
        public Color Color { get; set; }

        public CameraState Camera { get; set; } = CameraState.Visible;

        /// <summary>
        /// User Constructor 
        /// </summary>
        /// <param name="inputName">the name of the user</param>
        public User(string inputName)
        {
            Name = inputName;

            if (string.IsNullOrEmpty(inputName))
            {
                Color = Color.Gray;
            }
            else
            {
                var md5 = MD5.Create();
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(Name));
                Color = Color.FromArgb(hash[0], hash[1], hash[2]);
            }
        }

    }

}
