using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TagsCloud
{
	public class WordsTxtReader : IWordsReader
	{
		private readonly IErrorInformator errorInformator;

		public WordsTxtReader(IErrorInformator errorInformator)
		{
			this.errorInformator = errorInformator;
		}

		public List<string> ReadAllWords(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				errorInformator.PrintErrorMessage("Input file is not specified");
				errorInformator.Exit();
			}

			if (!File.Exists(fileName))
			{
				errorInformator.PrintErrorMessage($"Can't find input file \"{fileName}\"");
				errorInformator.Exit();
			}

			return File.ReadAllLines(fileName).ToList();
		}
	}
}