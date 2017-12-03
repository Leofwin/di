using System.Collections.Generic;
using System.Drawing;

namespace TagsCloud
{
	static class RectangleExtension
	{
		public static IEnumerable<Point> GetVertexes(this Rectangle rectangle)
		{
			return new List<Point>
			{
				rectangle.Location,
				new Point(rectangle.Right, rectangle.Top),
				new Point(rectangle.Left, rectangle.Bottom),
				new Point(rectangle.Right, rectangle.Bottom)
			};
		}
	}
}