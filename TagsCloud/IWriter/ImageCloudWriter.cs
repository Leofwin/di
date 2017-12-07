using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace TagsCloud
{
	public class ImageCloudWriter : ICloudWriter
	{
		private readonly IErrorInformator errorInformator;
		private readonly string outputFileName;

		public ImageCloudWriter(IErrorInformator errorInformator, string fileName)
		{
			this.errorInformator = errorInformator;
			outputFileName = fileName;
		}

		public void SaveCloud(Bitmap cloudImage)
		{
			try
			{
				cloudImage.Save(outputFileName, ImageFormat.Png);
			}
			catch (ArgumentException)
			{
				errorInformator.PrintErrorMessage("Can't write output file: incorrect argument");
			}
		}
	}
}