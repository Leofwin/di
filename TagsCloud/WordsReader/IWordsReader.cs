using System.Collections.Generic;

namespace TagsCloud
{
	public interface IWordsReader
	{
		Result<List<string>> ReadAllWords(string fileName);
	}
}
