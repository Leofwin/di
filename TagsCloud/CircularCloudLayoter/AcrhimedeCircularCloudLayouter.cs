using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TagsCloud
{
	public class AcrhimedeCircularCloudLayouter : ICircularCloudLayouter
	{
		private readonly ArhimedeSpiral spiral;
		public readonly List<Rectangle> Rectangles;
		public int RectanglesSquare { get; private set; }
		public double LengthFromCenterToFarthestPoint { get; private set; }
		private readonly Point center;

		public AcrhimedeCircularCloudLayouter(Point center)
		{
			this.center = center;
			Rectangles = new List<Rectangle>();

			const double deltaAngle = Math.PI / 180 / 2;
			const int spiralCoefficient = 1;

			spiral = new ArhimedeSpiral(deltaAngle, spiralCoefficient, center);
		}

		public Result<Rectangle> PutNextRectangle(Size rectangleSize)
		{
			if (rectangleSize.Width <= 0 || rectangleSize.Height <= 0)
				return Result.Fail<Rectangle>("Incorrect rectangle size");

			var result = Rectangles.Count == 0 
				? Geometry.CreateRectangle(center, rectangleSize)
				: GetNearestUnintersectedRectangleOnSpiral(rectangleSize);

			var lengthFromCenterToFarthestVertex = result.GetVertexes().Max(point => point.GetDistanceTo(center));
			if (lengthFromCenterToFarthestVertex > LengthFromCenterToFarthestPoint)
				LengthFromCenterToFarthestPoint = lengthFromCenterToFarthestVertex;

			RectanglesSquare += rectangleSize.Width * rectangleSize.Height;
			Rectangles.Add(result);
			return Result.Ok(result);
		}

		private Rectangle GetNearestUnintersectedRectangleOnSpiral(Size rectangleSize)
		{
			var result = new Rectangle();
			var isIntersect = true;
			while (isIntersect)
			{
				var centerPoint = spiral.GetNextPoint();
				result = Geometry.CreateRectangle(centerPoint, rectangleSize);

				isIntersect = Rectangles.Any(rectangle => result.IntersectsWith(rectangle));
			}
			return result;
		}
	}
}
