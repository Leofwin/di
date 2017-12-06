namespace TagsCloud
{
	public interface IWordFilter
	{
		bool IsValidateWord(string word);

		string GetFormatWord(string word);
	}
}
