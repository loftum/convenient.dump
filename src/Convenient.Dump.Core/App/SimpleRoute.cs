using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Convenient.Dump.Core.App
{
	internal class SimpleRoute
	{
		public string Method { get; }
		public Regex Path { get; }
		public Func<HttpContext, Match, Task<object>> Action { get; }

		public SimpleRoute(string method, string pathPattern, Func<HttpContext, Match, Task<object>> action)
		{
			Method = method;
			Path = new Regex(pathPattern, RegexOptions.IgnoreCase);
			Action = action;
		}

		public Func<HttpContext, Task<object>> GetAction(HttpContext context)
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