using System.Collections.Generic;
using System.Linq;

namespace TagsCloud
{
	public class WordFrequencySaver : IWordFrequencySaver
	{
		public Result<Dictionary<string, int>> GetWordsFreequency(
			Result<IEnumerable<string>> wordsResult, 
			int wordsCountLimit)
		{
			if (!wordsResult.IsSuccess)
				return Result.Fail<Dictionary<string, int>>(wordsResult.Error);

			var result = new Dictionary<string, int>();

			foreach (var word in wordsResult.Value)
			{
				if (!result.ContainsKey(word))
					result.Add(word, 0);
				result[word] += 1;
			}

			return Result.Ok(result
				.OrderByDescending(key => key.Value)
				.Take(wordsCountLimit)
				.ToDictionary(pair => pair.Key, pair => pair.Value));
		}
	}
}