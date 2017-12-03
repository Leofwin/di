using System.Collections.Generic;
using System.Drawing;

namespace TagsCloud
{
	public interface ICircularCloudLayouter
	{
		Rectangle PutNextRectangle(Size rectangleSize);
	}
}
