using System.Collections.Generic;

namespace TagsCloud
{
	public interface IWordsReader
	{
		List<string> ReadAllWords(string fileName);
	}
}
