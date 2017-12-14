using System.Drawing;

namespace TagsCloud
{
	public interface ICloudWriter
	{
		Result<None> SaveCloud(Result<Bitmap> cloudImageResult, string fileName);
	}
}
