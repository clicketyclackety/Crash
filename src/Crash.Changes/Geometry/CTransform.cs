using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

using Crash.Changes.Serialization;

namespace Crash.Geometry
{

	/// <summary>
	/// A Transformation Matrix.
	/// </summary>
	[JsonConverter(typeof(CTransformConverter))]
	public struct CTransform : IEnumerable<double>
	{

		private double[][] Transforms;

		public static CTransform Unset => new CTransform() { Transforms = GetUniformMatrix(double.NaN) };

		/// <summary>
		/// Defaults to 0 for all values.
		/// </summary>
		public CTransform()
		{
			Transforms = GetUniformMatrix(0);
		}

		public CTransform(double m00 = 0, double m01 = 0, double m02 = 0, double m03 = 0,
						 double m10 = 0, double m11 = 0, double m12 = 0, double m13 = 0,
						 double m20 = 0, double m21 = 0, double m22 = 0, double m23 = 0,
						 double m30 = 0, double m31 = 0, double m32 = 0, double m33 = 0)
			: this()
		{
			Transforms[0][0] = m00;
			Transforms[0][1] = m01;
			Transforms[0][2] = m02;
			Transforms[0][3] = m03;
			Transforms[1][0] = m10;
			Transforms[1][1] = m11;
			Transforms[1][2] = m12;
			Transforms[1][3] = m13;
			Transforms[2][0] = m20;
			Transforms[2][1] = m21;
			Transforms[2][2] = m22;
			Transforms[2][3] = m23;
			Transforms[3][0] = m30;
			Transforms[3][1] = m31;
			Transforms[3][2] = m32;
			Transforms[3][3] = m33;
		}

		public CTransform(params double[] mValues)
			: this()
		{
			for (int row = 0; row < 4; row++)
			{
				for (int col = 0; col < 4; col++)
				{
					int colValue = (row + 1) * (col + 1);
					if (col == 0 && row == col)
						colValue = 0;

					if (colValue >= mValues.Length) return;

					Transforms[row][col] = mValues[colValue];
				}
			}
		}

		private static double[][] GetUniformMatrix(double value)
		{
			var matrix = new double[][]
			{
					new double[4] { value, value, value, value },
					new double[4] { value, value, value, value },
					new double[4] { value, value, value, value },
					new double[4] { value, value, value, value },
			};
			return matrix;
		}

		public double this[int row, int column]
		{
			get => Transforms[row][column];
			set => Transforms[row][column] = value;
		}

		public bool IsValid()
		{
			double[] values = Transforms.SelectMany(t => t).ToArray();
			return values.Length == 16 &&
				!values.Any(v =>
				{
					return double.IsNaN(v) ||
							double.IsInfinity(v) ||
						   double.MaxValue == v ||
						   double.MinValue == v;
				});

		}

		public IEnumerator<double> GetEnumerator() => Transforms.SelectMany(v => v).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	}
}
