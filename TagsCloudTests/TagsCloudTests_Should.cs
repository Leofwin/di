﻿using System;
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
			textReader.ReadAllWords(fileName);

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
			var cloudMakerWithNormalFrequencySaver = new CloudMaker(
				wordsReader, wordFilter, new WordFrequencySaver(), fontNormalizer, tagsCloud, sizeDetector);
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

			sizeDetector.GetWordSize("", 0).ReturnsForAnyArgs(new Size(50, 50));
			tagsCloud.PutNextRectangle(new Size(0, 0)).ReturnsForAnyArgs(new Rectangle(0, 0, 10, 10));

			cloudMaker.GetRectanglesByWords(fontSizes);

			sizeDetector.Received(fontSizes.Count).GetWordSize(Arg.Any<string>(), Arg.Any<int>());
			tagsCloud.Received(fontSizes.Count).PutNextRectangle(Arg.Any<Size>());
		}
	}
}
