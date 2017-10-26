using System.Collections.Generic;

namespace ConvenientDump.LiteDb
{
	internal class TwoWayDictionary<TFirst, TSecond>
	{
		private readonly Dictionary<TFirst, TSecond> _first = new Dictionary<TFirst, TSecond>();
		private readonly Dictionary<TSecond, TFirst> _second = new Dictionary<TSecond, TFirst>();

		public TSecond this[TFirst key]
		{
			get => _first.TryGetValue(key, out var value) ? value : default(TSecond);
			set => _first[key] = value;
		}

		public TFirst this[TSecond key]
		{
			get => _second.TryGetValue(key, out var value) ? value : default(TFirst);
			set => _second[key] = value;
		}
	}
}