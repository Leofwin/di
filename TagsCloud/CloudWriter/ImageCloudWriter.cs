using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace TagsCloud
{
	public class ImageCloudWriter : ICloudWriter
	{
		public Result<None> SaveCloud(Result<Bitmap> cloudImageResult, string outputFileName)
		{
			if (!cloudImageResult.IsSuccess)
				return Result.Fail<None>(cloudImageResult.Error);

			if (string.IsNullOrEmpty(outputFileName))
				return Result.Fail<None>("Output file name is incorrect");

			try
			{
				cloudImageResult.Value.Save(outputFileName, ImageFormat.Png);
				return Result.Ok(new None());
			}
			catch (Exception)
			{
				return Result.Fail<None>($"Can't write image to file {outputFileName}");
			}
		}
	}
}