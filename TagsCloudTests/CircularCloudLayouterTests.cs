using System;
using System.Drawing;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TagsCloud;

namespace TagsCloudTests
{
	[TestFixture]
	public class CircularCloudLayouterTests_Should
	{
		private const double IsSimilarToCircle = 0.6;

		[TestCase(new[] { 100, 113 }, new[] { 40, 30 }, TestName = "TwoRectangles")]
		[TestCase(new[] { 100, 113, 300 }, new[] { 40, 30, 90 }, TestName = "ThreeRectangles")]
		[TestCase(new[] { 100, 113, 300, 189, 190 }, new[] { 40, 30, 90, 30, 110 }, TestName = "MoreThanThreeRectangles")]
		public void PutNextRectangle_SomeRectangles_AllShouldNotBeIntersected(int[] widths, int[] heights)
		{
			var tagsCloud = new AcrhimedeCircularCloudLayouter(new Point(0, 0));

			for (var i = 0; i < widths.Length; i++)
			{
				var actualResult = tagsCloud.PutNextRectangle(new Size(widths[i], heights[i]));

				actualResult.IsSuccess.Should().BeTrue();
				tagsCloud.Rectangles
					.Except(new[] {actualResult.Value})
					.Any(rectangle => rectangle.IntersectsWith(actualResult.Value))
					.Should().BeFalse();
			}
		}

		[Test]
		public void PutNextRectangle_SameRectangles_ShouldBeSimilarOnCircle()
		{
			var tagsCloud = new AcrhimedeCircularCloudLayouter(new Point(0, 0));
			var currentSize = new Size(24, 19);
			for (var i = 0; i < 300; i++)
				tagsCloud.PutNextRectangle(currentSize);

			currentSize = new Size(64, 15);
			for (var i = 0; i < 500; i++)
				tagsCloud.PutNextRectangle(currentSize);

			var circleSquare = Math.PI * Math.Pow(tagsCloud.LengthFromCenterToFarthestPoint, 2);
			var squareCoefficient = tagsCloud.RectanglesSquare / circleSquare;
			squareCoefficient.Should().BeGreaterThan(IsSimilarToCircle);
		}

		[Test]
		public void PutNextRectangle_RandomRectangles_ShouldBeSimilarOnCircle()
		{
			var tagsCloud = new AcrhimedeCircularCloudLayouter(new Point(0, 0));
			var random = new Random();
			for (var i = 0; i < 300; i++)
			{
				var width = random.Next(15, 67);
				var height = random.Next(10, 30);
				tagsCloud.PutNextRectangle(new Size(width, height));
			}
				

			var circleSquare = Math.PI * Math.Pow(tagsCloud.LengthFromCenterToFarthestPoint, 2);
			var squareCoefficient = tagsCloud.RectanglesSquare / circleSquare;
			squareCoefficient.Should().BeGreaterThan(IsSimilarToCircle);
		}

		[TestCase(50, 50, 0, 0, -25, -25, TestName = "IfCentralPointIsZero")]
		[TestCase(76, 34, 40, 40, 2, 23, TestName = "IfCentralPointIsNotZero")]
		public void PutNextRectangle_FirstRectangle(
			int width, int height, int centerX, int centerY, int expectedX, int expectedY)
		{
			var tagsCloud = new AcrhimedeCircularCloudLayouter(new Point(centerX, centerY));
			var expected = new Rectangle(expectedX, expectedY, width, height);

			var actualResult = tagsCloud.PutNextRectangle(new Size(width, height));

			actualResult.IsSuccess.Should().BeTrue();
			actualResult.Value.Should().Be(expected);
		}

		[TestCase(-20, 3, TestName = "IfWidthIsNegative")]
		[TestCase(150, -30, TestName = "IfHeightIsNegative")]
		[TestCase(0, 5, TestName = "IfWidthIsZero")]
		[TestCase(10, 0, TestName = "IfHeightIsZero")]
		public void PutNextRectangle_IfIncorrectSize_ResultFail(int width, int height)
		{
			var tagsCloud = new AcrhimedeCircularCloudLayouter(new Point(0, 0));

			var result = tagsCloud.PutNextRectangle(new Size(width, height));

			result.IsSuccess.Should().BeFalse();
			result.Error.Should().NotBeNullOrEmpty();
		}
	}
}
