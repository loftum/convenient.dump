using System;
using System.Collections.Generic;

namespace ConvenientDump.LiteDb
{
	internal class TypeCodeConverter
	{
		private readonly TwoWayDictionary<Type, int> _typeCodes = new TwoWayDictionary<Type, int>();
		private readonly Dictionary<int, Func<string, object>> _converters = new Dictionary<int, Func<string, object>>();

		public TypeCodeConverter()
		{
			this[typeof(string), 0] = v => v;
			this[typeof(int), 1] = v => int.TryParse(v, out var i) ? i : 0;
		}

		public Func<string, object> this[Type type, int code]
		{
			set
			{
				_typeCodes[type] = code;
				_converters[code] = value;
			}
		}

		public int this[Type type] => _typeCodes[type];

		public Func<string, object> this[int code] => _converters.TryGetValue(code, out var func) ? func : v => v;
	}
}