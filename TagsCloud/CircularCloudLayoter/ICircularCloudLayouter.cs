using System.Drawing;

namespace TagsCloud
{
	public interface ICircularCloudLayouter
	{
		Rectangle PutNextRectangle(Size rectangleSize);
	}
}
