using System.Drawing;

namespace TagsCloud
{
	public interface ISizeDetector
	{
		Result<Size> GetWordSize(string word, int fontSize);
	}
}
