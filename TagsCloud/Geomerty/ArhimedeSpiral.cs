using System;
using System.Drawing;

namespace TagsCloud
{
	public class ArhimedeSpiral
	{
		private double currentArgument;
		public readonly double DeltaAngle;
		public readonly double SpiralCoefficient;
		public readonly Point Center;

		public ArhimedeSpiral(double deltaAngle, double coefficient, Point center)
		{
			DeltaAngle = deltaAngle;
			SpiralCoefficient = coefficient;
			Center = center;
		}

		public Point GetNextPoint()
		{
			var result = new Point(
				Center.X + (int)(currentArgument * SpiralCoefficient * Math.Cos(currentArgument)),
				Center.Y + (int)(currentArgument * SpiralCoefficient * Math.Sin(currentArgument))
			);
			currentArgument += DeltaAngle;
			return result;
		}

	}
}