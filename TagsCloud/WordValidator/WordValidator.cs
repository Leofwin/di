namespace TagsCloud
{
	public class WordValidator : IWordValidator
	{
		public bool IsValidateWord(string word)
		{
			return word.Length > 4;
		}
	}
}