namespace Crash.Changes.Serialization
{
	internal static class FloatingDoubleConverter
	{

		const string NaN = "NaN";
		const string PositiveInfinity = "+∞";
		const string NegativeInfinity = "-∞";

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

			if (double.TryParse(number, out double result))
				return result;

			return 0;
		}

	}
}
