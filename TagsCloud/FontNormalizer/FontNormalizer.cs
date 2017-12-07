namespace TagsCloud
{
	public class FontNormalizer : IFontNormalizer
	{
		private readonly int minFontSize;
		private readonly int maxFontSize;

		public FontNormalizer(IErrorInformator errorInformator, int minFontSize, int maxFontSize)
		{
			if (minFontSize <= 0 || maxFontSize <= 0)
			{
				errorInformator.PrintErrorMessage("Font sizes shoud be positive numbers");
				errorInformator.Exit();
			}

			if (minFontSize > maxFontSize)
			{
				errorInformator.PrintErrorMessage("Max font size should be more than min font size");
				errorInformator.Exit();
			}

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