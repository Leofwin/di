using System.Drawing;
using System.Linq;

namespace TagsCloud
{
	public class CloudMaker
	{
		private readonly IWordFrequencySaver wordFrequencySaver;
		private readonly IFontNormalizer fontNormalizer;
		private readonly ICircularCloudLayouter tagsCloud;
		private readonly ISizeDetector sizeDetector;

		public CloudMaker(
			IWordFrequencySaver wordFrequencySaver, 
			IFontNormalizer fontNormalizer,
			ICircularCloudLayouter tagsCloud,
			ISizeDetector sizeDetector)
		{
			this.wordFrequencySaver = wordFrequencySaver;
			this.fontNormalizer = fontNormalizer;
			this.tagsCloud = tagsCloud;
			this.sizeDetector = sizeDetector;
		}

		public Bitmap GenerateImage(string input, Size imageSize, 
			Color fontColor, FontFamily fontFamily, int wordsCount)
		{
			var frequencyByWords = wordFrequencySaver
				.GetWordsFreequency(input, wordsCount);

			var maxFrequency = frequencyByWords.Max(p => p.Value);
			var minFrequency = frequencyByWords.Min(p => p.Value);

			var fontSizeByWords = frequencyByWords
				.ToDictionary(p => p.Key, p => fontNormalizer.GetFontSize(p.Value, maxFrequency, minFrequency));

			var sizesByWords = fontSizeByWords
				.ToDictionary(p => p.Key, p => sizeDetector.GetWordSize(p.Key, p.Value));

			var rectanglesByWords = sizesByWords
				.ToDictionary(p => p.Key, p => tagsCloud.PutNextRectangle(p.Value));

			var bitmap = new Bitmap(imageSize.Width, imageSize.Height);
			var brush = new SolidBrush(fontColor);
			var pen = new Pen(Color.White);

			var g = Graphics.FromImage(bitmap);
			g.FillRectangle(new SolidBrush(Color.White), 0, 0, bitmap.Width, bitmap.Height);

			foreach (var pair in rectanglesByWords)
			{
				g.DrawRectangle(pen, pair.Value);
				g.DrawString(pair.Key, new Font(fontFamily, fontSizeByWords[pair.Key]), brush, pair.Value);
			}

			return bitmap;
		}
	}
}
