using System.Collections.Generic;

namespace TagsCloud
{
	public interface IWordFrequencySaver
	{
		Result<Dictionary<string, int>> GetWordsFreequency(
			Result<IEnumerable<string>> wordsResult, 
			int wordCountLimit);
	}
}
