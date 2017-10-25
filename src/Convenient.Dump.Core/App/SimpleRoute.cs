using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Convenient.Dump.Core.App
{
	internal class SimpleRoute
	{
		public string Method { get; }
		public Regex Path { get; }
		public Func<SimpleContext, Match, Task<object>> Action { get; }

		public SimpleRoute(string method, string pathPattern, Func<SimpleContext, Match, Task<object>> action)
		{
			Method = method;
			Path = new Regex(pathPattern, RegexOptions.IgnoreCase);
			Action = action;
		}

		public Func<SimpleContext, Task<object>> GetAction(SimpleContext context)
		{
			if (context == null || context.Request.Method != Method)
			{
				return null;
			}
			var match = Path.Match(context.Request.Path);
			if (match.Success)
			{
				return c => Action(c, match);
			}
			return null;
		}
	}
}