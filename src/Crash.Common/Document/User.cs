﻿using System.Drawing;
using System.Security.Cryptography;
using System.Text;


namespace Crash.Common.Document
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
	public struct User : IEquatable<User>
	{
		private string _name;

		public bool Visible { get; set; } = true;

		/// <summary>
		/// Name of the user
		/// </summary>
		public string Name
		{
			get => _name;
			set => _name = CleanedUserName(value);
		}

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
			_name = CleanedUserName(inputName.ToLower());

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


		public bool IsValid() => !string.IsNullOrEmpty(Name);

		public override int GetHashCode() => string.IsNullOrEmpty(Name) ? -1 : Name.GetHashCode();

		public override bool Equals(object obj)
		{
			if (obj is not User user) return false;
			return Equals(user);
		}

		public bool Equals(User other) => this.GetHashCode() == other.GetHashCode();

		// TODO : Add cleaning methods
		public static string CleanedUserName(string username) => username.ToLower();

	}

}
