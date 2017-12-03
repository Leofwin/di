using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TagsCloud
{
	public class WordsTxtReader : IWordsReader
	{
		public List<string> ReadAllWords(string fileName)
		{
			return File.ReadAllLines(fileName).ToList();

		}
	}
}