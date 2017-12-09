using System;
using System.Collections.Generic;
using System.Linq;

namespace TagsCloud
{
	public class FiltersKeeper
	{
		private static readonly Dictionary<string, (Func<string, bool>, string)> AllFilters = 
			new Dictionary<string, (Func<string, bool>, string)>
			{
				{"length>4", (w => w.Length > 4, "Words should have more than 4 letters") }
			};

		public static Dictionary<string, string> GetFilterDescription()
		{
			return AllFilters.ToDictionary(p => p.Key, p => p.Value.Item2);
		}

		public static Func<string, bool> GetFilterByName(string name)
		{
			if (!AllFilters.ContainsKey(name))
				throw new ArgumentException("This filters doesn't exist");

			return AllFilters[name].Item1;
		}
	}
}
