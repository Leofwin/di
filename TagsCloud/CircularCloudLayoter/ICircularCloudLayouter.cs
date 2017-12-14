using System.Drawing;

namespace TagsCloud
{
	public interface ICircularCloudLayouter
	{
		Result<Rectangle> PutNextRectangle(Size rectangleSize);
	}
}
