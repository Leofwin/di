using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using TagsCloud;

namespace TagsCloudTests
{
	[TestFixture]
	public class TagsCloudTests_Should
	{
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
		public void FontNormalizer_IfBadFontSizes_Exception(int minFontSize, int maxFontSize)
		{
			Action action = () => new FontNormalizer(minFontSize, maxFontSize);
			action.ShouldThrow<ArgumentException>();
		}

		[TestCase("", TestName = "IfEmptyString")]
		[TestCase(null, TestName = "IfNull")]
		public void ImageCloudWriter_IncorrectOutputFileName_ResultFail(string output)
		{
			var imageCloudWriter = new ImageCloudWriter();
			var result = imageCloudWriter.SaveCloud(Result.Ok(new Bitmap(100, 100)), output);

			result.IsSuccess.Should().BeFalse();
			result.Error.Should().NotBeNullOrEmpty();
		}

		[TestCase("abcd", TestName = "IfDosNotContainFilterName")]
		[TestCase("doesNotExistingFilter", TestName = "IfDosNotContainFilterName")]
		public void FiltersKeeper_GetFilterByName_IfFilterDoesntExist_ResultFail(string filter)
		{
			var result = FiltersKeeper.GetFilterByName(filter);

			result.IsSuccess.Should().BeFalse();
			result.Error.Should().NotBeNullOrEmpty();
		}

		[Test]
		public void FiltersKeeper_GetFilterByName_IfNullFilterName_Exception()
		{
			Action action = () => FiltersKeeper.GetFilterByName(null);
			action.ShouldThrow<ArgumentException>();
		}

		[TestCase(null, TestName = "IfFileNameIsNull")]
		[TestCase("", TestName = "IfEmptyFileName")]
		[TestCase("notExistFile.tmp", TestName = "IfFileDoesNotExist")]
		public void TextReader_IfBadFileWithBoringWordsName_ResultFail(string fileName)
		{
			var textReader = new TextReader();
			var result = textReader.ReadAllWords(fileName);

			result.IsSuccess.Should().BeFalse();
			result.Error.Should().NotBeNullOrEmpty();
		}

		[TestCase(null, TestName = "IfFileNameIsNull")]
		[TestCase("", TestName = "IfEmptyFileName")]
		[TestCase("notExistFile.tmp", TestName = "IfFileDoesNotExist")]
		public void WordsTextReader_IfBadFileWithBoringWordsName_PrintInfoMessage(string fileName)
		{
			var textReader = new TextReader();

			var result = textReader.ReadAllWords(fileName);

			result.IsSuccess.Should().BeFalse();
			result.Error.Should().NotBeNullOrEmpty();
		}

		[Test]
		public void CloudMaker_GetFontSizeByWords_CountOfCallsMethodsShouldBeEqualWordsCount()
		{
			var cloudMakerWithNormalFrequencySaver = new CloudMaker(
				wordsReader, wordFilter, new WordFrequencySaver(), fontNormalizer, tagsCloud, sizeDetector);
			var words = new[] {"hello", "world", "glad", "to", "see", "you" };
			var frequency = new Dictionary<string, int>
			{
				{ "word",words.Length}
			};

			wordsReader.ReadAllWords(Arg.Any<string>())
				.Returns(Result.Ok(words.ToList()));
			wordFilter.GetFormatWord(Arg.Any<string>())
				.Returns("word");
			wordFilter.IsValidateWord(Arg.Any<string>())
				.Returns(true);
			fontNormalizer.GetFontSize(10, 10, 10)
				.ReturnsForAnyArgs(10);

			cloudMakerWithNormalFrequencySaver.GetFontSizeByWords("test", words.Length);

			wordsReader.Received(1).ReadAllWords(Arg.Any<string>());
			wordFilter.Received(words.Length).GetFormatWord(Arg.Any<string>());
			wordFilter.Received(words.Length).IsValidateWord(Arg.Any<string>());
			fontNormalizer.Received(frequency.Count).GetFontSize(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>());
		}

		[Test]
		public void CloudMaker_GetRectanglesByWords_CountOfMethodCallsShouldBeEqualUniqueWordsCount()
		{
			var fontSizes = new Dictionary<string, int>
			{
				{"hello", 32 },
				{"big", 20 },
				{"world", 16 },
				{"again", 12 }
			};

			sizeDetector.GetWordSize("", 0).ReturnsForAnyArgs(Result.Ok(new Size(50, 50)));
			tagsCloud.PutNextRectangle(new Size(0, 0)).ReturnsForAnyArgs(Result.Ok(new Rectangle(0, 0, 10, 10)));

			cloudMaker.GetRectanglesByWords(Result.Ok(fontSizes));

			sizeDetector.Received(fontSizes.Count).GetWordSize(Arg.Any<string>(), Arg.Any<int>());
			tagsCloud.Received(fontSizes.Count).PutNextRectangle(Arg.Any<Size>());
		}
	}
}
