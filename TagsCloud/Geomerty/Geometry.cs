using System.Drawing;

namespace TagsCloud
{
	static class Geometry
	{
		public static Rectangle CreateRectangle(Point centerPoint, Size size)
		{
			var topUpperPoint = GetUpperLeftPointByCenterOfRectangle(centerPoint, size);
			return new Rectangle(topUpperPoint.X, topUpperPoint.Y, size.Width, size.Height);
		}

		public static Point GetUpperLeftPointByCenterOfRectangle(Point rectangleCenter, Size rectangleSize)
		{
			return new Point(
				rectangleCenter.X - rectangleSize.Width / 2,
				rectangleCenter.Y - rectangleSize.Height / 2
			);
		}
	}
}