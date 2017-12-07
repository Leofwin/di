using System;
using System.Collections.Generic;
using System.Linq;

namespace TagsCloud
{
	public class FiltersKeeper
	{
		private static readonly Dictionary<string, (Func<string, bool>, string)> allFilters = 
			new Dictionary<string, (Func<string, bool>, string)>
			{
				{"length>4", (w => w.Length > 4, "Words should have more than 4 letters") }
			};

		public static Dictionary<string, string> GetFilterDescription()
		{
			return allFilters.ToDictionary(p => p.Key, p => p.Value.Item2);
		}

		public static Func<string, bool> GetFilterByName(string name)
		{
			if (!allFilters.ContainsKey(name))
				throw new ArgumentException("This filters doesn't exist");

			return allFilters[name].Item1;
		}
	}
}
