using System.Collections.Generic;
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
		private readonly IWordsReader wordsReader;
		private readonly IWordFilter wordFilter;

		public CloudMaker(
			IWordsReader wordsReader,
			IWordFilter wordFilter,
			IWordFrequencySaver wordFrequencySaver, 
			IFontNormalizer fontNormalizer,
			ICircularCloudLayouter tagsCloud,
			ISizeDetector sizeDetector)
		{
			this.wordFrequencySaver = wordFrequencySaver;
			this.fontNormalizer = fontNormalizer;
			this.tagsCloud = tagsCloud;
			this.sizeDetector = sizeDetector;
			this.wordsReader = wordsReader;
			this.wordFilter = wordFilter;
		}

		public Bitmap GenerateImage(Size imageSize, Color fontColor, FontFamily fontFamily,
			Dictionary<string, int> fontSizeByWords, Dictionary<string, Rectangle> rectanglesByWords)
		{
			var bitmap = new Bitmap(imageSize.Width, imageSize.Height);
			var brush = new SolidBrush(fontColor);
			var pen = new Pen(Color.White);

			using (var g = Graphics.FromImage(bitmap)) { 
				g.FillRectangle(new SolidBrush(Color.White), 0, 0, bitmap.Width, bitmap.Height);

				foreach (var pair in rectanglesByWords)
				{
					g.DrawRectangle(pen, pair.Value);
					g.DrawString(pair.Key, new Font(fontFamily, fontSizeByWords[pair.Key]), brush, pair.Value);
				}
			}

			return bitmap;
		}

		public Dictionary<string, Rectangle> GetRectanglesByWords(Dictionary<string, int> fontSizeByWords)
		{
			var sizesByWords = fontSizeByWords
				.ToDictionary(p => p.Key, p => sizeDetector.GetWordSize(p.Key, p.Value));

			var rectanglesByWords = sizesByWords
				.ToDictionary(p => p.Key, p => tagsCloud.PutNextRectangle(p.Value));
			return rectanglesByWords;
		}

		public Dictionary<string, int> GetFontSizeByWords(string input, int wordsCount)
		{
			var words = wordsReader.ReadAllWords(input)
				.Select(word => wordFilter.GetFormatWord(word))
				.Where(word => wordFilter.IsValidateWord(word))
				.ToList(); // Необходимо для прохождения одного из тестов

			var frequencyByWords = wordFrequencySaver
				.GetWordsFreequency(words, wordsCount);

			var maxFrequency = frequencyByWords.Max(p => p.Value);
			var minFrequency = frequencyByWords.Min(p => p.Value);

			return frequencyByWords
				.ToDictionary(p => p.Key, p => fontNormalizer.GetFontSize(p.Value, maxFrequency, minFrequency));
		}
	}
}
