using System;
using System.Drawing;

namespace TagsCloud
{
	static class PointExtension
	{
		public static double GetDistanceTo(this Point start, Point end)
		{
			return Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2));
		}	
	}
}