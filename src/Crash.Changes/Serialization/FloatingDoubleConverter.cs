namespace Crash.Changes.Serialization
{

	/// <summary>Utility for converting strings to numbers</summary>
	internal static class FloatingDoubleConverter
	{

		const string NaN = "NaN";
		const string PositiveInfinity = "+∞";
		const string NegativeInfinity = "-∞";

		/// <summary>Converts the number to string</summary>
		internal static string ToString(double number)
		{
			if (double.IsNaN(number))
				return NaN;

			if (double.IsPositiveInfinity(number))
				return PositiveInfinity;

			if (double.IsNegativeInfinity(number))
				return NegativeInfinity;

			return number.ToString();
		}

		/// <summary>Converts the number from string</summary>
		internal static double FromString(string? number)
		{
			if (string.IsNullOrEmpty(number))
				return 0;

			if (number.Equals(NaN))
				return double.NaN;

			if (number.Equals(PositiveInfinity))
				return double.PositiveInfinity;

			if (number.Equals(NegativeInfinity))
				return double.NegativeInfinity;

			if (double.TryParse(number, out var result))
				return result;

			return 0;
		}

	}
}
