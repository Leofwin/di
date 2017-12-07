using System.Collections.Generic;
using System.Linq;

namespace TagsCloud
{
	public class WordFrequencySaver : IWordFrequencySaver
	{
		public Dictionary<string, int> GetWordsFreequency(IEnumerable<string> words, int wordsCountLimit)
		{
			var result = new Dictionary<string, int>();

			foreach (var word in words)
			{
				if (!result.ContainsKey(word))
					result.Add(word, 0);
				result[word] += 1;
			}

			return result
				.OrderByDescending(key => key.Value)
				.Take(wordsCountLimit)
				.ToDictionary(pair => pair.Key, pair => pair.Value);
		}
	}
}