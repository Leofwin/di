namespace TagsCloud
{
	public class FontNormalizer : IFontNormalizer
	{
		private readonly int minFontSize;
		private readonly int maxFontSize;

		public FontNormalizer(int minFontSize, int maxFontSize)
		{
			this.minFontSize = minFontSize;
			this.maxFontSize = maxFontSize;
		}
		public int GetFontSize(int frequency, int maxFrequency, int minFrequency)
		{
			var coefficient = (double)(maxFontSize - minFontSize) / (maxFrequency - minFrequency);
			return (int)(minFontSize + (frequency - minFrequency) * coefficient);
		}
	}
}