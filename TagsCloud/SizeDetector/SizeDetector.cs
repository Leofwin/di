using System.Drawing;

namespace TagsCloud
{
	public class SizeDetector : ISizeDetector
	{
		public Result<Size> GetWordSize(string word, int fontSize)
		{
			if (fontSize < 0)
				return Result.Fail<Size>("Font size should be positive");

			if (string.IsNullOrEmpty(word))
				return Result.Fail<Size>("Incorrect word");

			return Result.Ok(
				new Size(fontSize * word.Length, 
				(int)(fontSize * 1.5))
			);
		}
	}
}