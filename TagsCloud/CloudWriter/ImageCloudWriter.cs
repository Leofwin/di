using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace TagsCloud
{
	public class ImageCloudWriter : ICloudWriter
	{
		private readonly IErrorInformator errorInformator;

		public ImageCloudWriter(IErrorInformator errorInformator)
		{
			this.errorInformator = errorInformator;
		}

		public void SaveCloud(Bitmap cloudImage, string outputFileName)
		{
			if (string.IsNullOrEmpty(outputFileName))
			{
				errorInformator.PrintErrorMessage("Output file name is incorrect");
				errorInformator.Exit();
			}

			try
			{
				cloudImage.Save(outputFileName, ImageFormat.Png);
			}
			catch (Exception)
			{
				errorInformator.PrintErrorMessage($"Can't write image to file {outputFileName}");
				errorInformator.Exit();
			}
		}
	}
}