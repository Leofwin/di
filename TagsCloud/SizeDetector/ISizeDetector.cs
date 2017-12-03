using System.Drawing;

namespace TagsCloud
{
	public interface ISizeDetector
	{
		Size GetWordSize(string word, int fontSize);
	}
}
