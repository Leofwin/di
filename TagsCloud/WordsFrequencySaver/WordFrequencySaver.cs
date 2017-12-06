using System.Collections.Generic;
using System.Linq;

namespace TagsCloud
{
	public class WordFrequencySaver : IWordFrequencySaver
	{
		private readonly IWordsReader wordsReader;
		private readonly IWordFilter wordFilter;

		public WordFrequencySaver(
			IWordsReader wordsReader, 
			IWordFilter wordFilter)
		{
			this.wordsReader = wordsReader;
			this.wordFilter = wordFilter;
		}
		public Dictionary<string, int> GetWordsFreequency(string fileName, int wordsCountLimit)
		{
			var result = new Dictionary<string, int>();

			var words = wordsReader.ReadAllWords(fileName)
				.Select(word => wordFilter.GetFormatWord(word))
				.Where(word => wordFilter.IsValidateWord(word));

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