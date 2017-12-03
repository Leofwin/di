using System.Drawing;

namespace TagsCloud
{
	public class SizeDetector : ISizeDetector
	{
		public Size GetWordSize(string word, int fontSize)
		{
			return new Size(
				fontSize * word.Length, 
				(int)(fontSize * 1.5)
			);
		}
	}
}