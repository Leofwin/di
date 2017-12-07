using System.Collections.Generic;

namespace TagsCloud
{
	public interface IWordFrequencySaver
	{
		Dictionary<string, int> GetWordsFreequency(IEnumerable<string> words, int wordCountLimit);
	}
}
