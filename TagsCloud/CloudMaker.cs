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

		public Result<Bitmap> GenerateImage(Size imageSize, Color fontColor, Result<Font> fontResult,
			Result<Dictionary<string, int>> fontSizeByWordsResult, 
			Result<Dictionary<string, Rectangle>> rectanglesByWordsResult)
		{
			if (!fontSizeByWordsResult.IsSuccess)
				return Result.Fail<Bitmap>(fontSizeByWordsResult.Error);

			if (!rectanglesByWordsResult.IsSuccess)
				return Result.Fail<Bitmap>(rectanglesByWordsResult.Error);

			if (!fontResult.IsSuccess)
				return Result.Fail<Bitmap>(fontResult.Error);

			var bitmap = new Bitmap(imageSize.Width, imageSize.Height);
			var brush = new SolidBrush(fontColor);
			var pen = new Pen(Color.White);

			using (var g = Graphics.FromImage(bitmap)) { 
				g.FillRectangle(new SolidBrush(Color.White), 0, 0, bitmap.Width, bitmap.Height);

				foreach (var pair in rectanglesByWordsResult.Value)
				{
					g.DrawRectangle(pen, pair.Value);
					g.DrawString(
						pair.Key, 
						new Font(
							fontResult.Value.FontFamily, 
							fontSizeByWordsResult.Value[pair.Key]
						), 
						brush, 
						pair.Value);
				}
			}

			return Result.Ok(bitmap);
		}

		public Result<Dictionary<string, Rectangle>> GetRectanglesByWords(
			Result<Dictionary<string, int>> fontSizeByWordsResult)
		{
			return fontSizeByWordsResult
				.ToDictionary(p => Result.Ok(p.Key), p => sizeDetector.GetWordSize(p.Key, p.Value))
				.ToDictionary(p => Result.Ok(p.Key), p => tagsCloud.PutNextRectangle(p.Value));
		}

		public Result<Dictionary<string, int>> GetFontSizeByWords(string input, int wordsCount)
		{
			var wordsResult = wordsReader.ReadAllWords(input)
				.Then(r => r.Value
					.Select(word => wordFilter.GetFormatWord(word))
					.Where(word => wordFilter.IsValidateWord(word))
				);

			return wordFrequencySaver
				.GetWordsFreequency(wordsResult, wordsCount)
				.Then(r => FrequencyToFontSize(r.Value));
		}

		private Dictionary<string, int> FrequencyToFontSize(Dictionary<string, int> frequncy)
		{
			var maxFrequency = frequncy.Max(p => p.Value);
			var minFrequency = frequncy.Min(p => p.Value);

			return frequncy.ToDictionary(
				p => p.Key, 
				p => fontNormalizer.GetFontSize(p.Value, maxFrequency, minFrequency)
			);
		}
	}
}
