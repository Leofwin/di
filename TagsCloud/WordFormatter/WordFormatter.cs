namespace TagsCloud
{
	public class WordFormatter : IWordFormatter
	{
		public string GetFormatWord(string word)
		{
			return word.ToLower();
		}
	}
}