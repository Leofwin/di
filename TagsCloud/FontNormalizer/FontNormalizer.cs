using System;

namespace TagsCloud
{
	public class FontNormalizer : IFontNormalizer
	{
		private readonly int minFontSize;
		private readonly int maxFontSize;

		public FontNormalizer(int minFontSize, int maxFontSize)
		{
			if (minFontSize <= 0 || maxFontSize <= 0)
				throw new ArgumentException("Font sizes should be positive");

			if (minFontSize > maxFontSize)
				throw new ArgumentException("Min font size should be less than max font size");

			this.minFontSize = minFontSize;
			this.maxFontSize = maxFontSize;
		}
		public int GetFontSize(int frequency, int maxFrequency, int minFrequency)
		{
			if (maxFrequency == minFrequency)
				return maxFontSize;
			var coefficient = (double)(maxFontSize - minFontSize) / (maxFrequency - minFrequency);
			return (int)(minFontSize + (frequency - minFrequency) * coefficient);
		}
	}
}