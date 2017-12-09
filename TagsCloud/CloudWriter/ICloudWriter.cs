using System.Drawing;

namespace TagsCloud
{
	public interface ICloudWriter
	{
		void SaveCloud(Bitmap cloudImage, string fileName);
	}
}
