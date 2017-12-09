using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using TagsCloud;

namespace TagsCloudTests
{
	[TestFixture]
	public class TagsCloudTests_Should
	{
		private IErrorInformator errorInformator;
		private IWordFrequencySaver wordFrequencySaver;
		private IFontNormalizer fontNormalizer;
		private ICircularCloudLayouter tagsCloud;
		private ISizeDetector sizeDetector;
		private IWordsReader wordsReader;
		private IWordFilter wordFilter;
		private CloudMaker cloudMaker;

		[SetUp]
		public void SetUp()
		{
			errorInformator = Substitute.For<IErrorInformator>();
			wordFrequencySaver = Substitute.For<IWordFrequencySaver>();
			fontNormalizer = Substitute.For<IFontNormalizer>();
			tagsCloud = Substitute.For<ICircularCloudLayouter>();
			sizeDetector = Substitute.For<ISizeDetector>();
			wordsReader = Substitute.For<IWordsReader>();
			wordFilter = Substitute.For<IWordFilter>();
			cloudMaker = new CloudMaker(wordsReader, wordFilter, wordFrequencySaver, 
				fontNormalizer, tagsCloud, sizeDetector);
		}

		[TestCase(-5, 20, TestName = "IfMinSizeIsNegative")]
		[TestCase(0, 20, TestName = "IfMinSizeIsZero")]
		[TestCase(5, -10, TestName = "IfMaxSizeIsNegative")]
		[TestCase(5, 0, TestName = "IfMaxSizeIsZero")]
		[TestCase(45, 20, TestName = "IfMaxSizeIsZero")]
		public void FontNormalizer_IfBadFontSizes_PrintErrorMessageAndExit(int minFontSize, int maxFontSize)
		{
			new FontNormalizer(errorInformator, minFontSize, maxFontSize);

			errorInformator.Received().PrintErrorMessage(Arg.Any<string>());
			errorInformator.Received().Exit();
		}

		[TestCase("", TestName = "IfEmptyString")]
		[TestCase(null, TestName = "IfNull")]
		public void ImageCloudWriter_IncorrectOutputFileName_PrintErrorMessageAndExit(string output)
		{
			var imageCloudWriter = new ImageCloudWriter(errorInformator);
			imageCloudWriter.SaveCloud(new Bitmap(100, 100), output);

			errorInformator.Received().PrintErrorMessage(Arg.Any<string>());
			errorInformator.Received().Exit();
		}

		[Test]
		public void ImageCloudWriter_IfNullBitmap_PrintErrorMessageAndExit()
		{
			var imageCloudWriter = new ImageCloudWriter(errorInformator);
			imageCloudWriter.SaveCloud(null, "result.png");

			errorInformator.Received().PrintErrorMessage(Arg.Any<string>());
			errorInformator.Received().Exit();
		}

		[TestCase(null, TestName = "IfNull")]
		[TestCase("abcd", TestName = "IfDosNotContainFilterName")]
		public void FiltersKeeper_GetFilterByName_IfBadFilterName_Exception(string filter)
		{
			Assert.Catch<ArgumentException>(() => FiltersKeeper.GetFilterByName(filter));
		}

		[TestCase(null, TestName = "IfFileNameIsNull")]
		[TestCase("", TestName = "IfEmptyFileName")]
		[TestCase("notExistFile.tmp", TestName = "IfFileDoesNotExist")]
		public void WordFilter_IfBadFileWithBoringWordsName_PrintInfoMessage(string fileName)
		{
			new WordFilter(errorInformator, fileName, new string[0]);

			errorInformator.Received().PrintInfoMessage(Arg.Any<string>());
		}

		[TestCase(null, TestName = "IfFileNameIsNull")]
		[TestCase("", TestName = "IfEmptyFileName")]
		[TestCase("notExistFile.tmp", TestName = "IfFileDoesNotExist")]
		public void TextReader_IfBadFileWithBoringWordsName_PrintInfoMessage(string fileName)
		{
			var textReader = new TextReader(errorInformator);

			try
			{
				textReader.ReadAllWords(fileName);
			}
			catch (Exception)
			{
				// ignored
			}

			errorInformator.Received().PrintErrorMessage(Arg.Any<string>());
			errorInformator.Received().Exit();
		}

		[TestCase(null, TestName = "IfFileNameIsNull")]
		[TestCase("", TestName = "IfEmptyFileName")]
		[TestCase("notExistFile.tmp", TestName = "IfFileDoesNotExist")]
		public void WordsTextReader_IfBadFileWithBoringWordsName_PrintInfoMessage(string fileName)
		{
			var textReader = new TextReader(errorInformator);

			try
			{
				textReader.ReadAllWords(fileName);
			}
			catch (Exception)
			{
				// ignored
			}

			errorInformator.Received().PrintErrorMessage(Arg.Any<string>());
			errorInformator.Received().Exit();
		}

		[Test]
		public void CloudMaker_GetFontSizeByWords_CountOfCallsMethodsShouldBeEqualWordsCount()
		{
			var words = new[] {"hello", "world", "glad", "to", "see", "you" };
			var frequency = new Dictionary<string, int>
			{
				{ "word",words.Length}
			};

			wordsReader.ReadAllWords(Arg.Any<string>())
				.Returns(words.ToList());
			wordFilter.GetFormatWord(Arg.Any<string>())
				.Returns("word");
			wordFilter.IsValidateWord(Arg.Any<string>())
				.Returns(true);
			fontNormalizer.GetFontSize(10, 10, 10)
				.ReturnsForAnyArgs(10);
			wordFrequencySaver.GetWordsFreequency(new string[0], 10)
				.ReturnsForAnyArgs(frequency);

			cloudMaker.GetFontSizeByWords("test", words.Length);

			wordsReader.Received(1).ReadAllWords(Arg.Any<string>());
			wordFilter.Received(words.Length).GetFormatWord(Arg.Any<string>());
			wordFilter.Received(words.Length).IsValidateWord(Arg.Any<string>());
			wordFrequencySaver.Received(1).GetWordsFreequency(
				Arg.Any<IEnumerable<string>>(), 
				Arg.Any<int>()
			);
			fontNormalizer.Received(frequency.Count).GetFontSize(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>());
		}
	}
}
