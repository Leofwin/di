using System.Collections.Generic;

namespace TagsCloud
{
	public interface IWordFrequencySaver
	{
		Dictionary<string, int> GetWordsFreequency(string fileName, int wordsCountLimit);
	}
}
