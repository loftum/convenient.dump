using System;
using System.IO;

namespace Convenient.Dump.Core.App.IO
{
	internal class FileThingy
	{
		public DateTimeOffset ReadTime { get; }
		public string Name { get; }
		public string Content { get; }
		public bool IsEmbedded { get; }

		public bool IsUpToDate()
		{
			return IsEmbedded || ReadTime > File.GetLastWriteTimeUtc(Name);
		}

		public FileThingy(string name, string content, bool isEmbedded)
		{
			Name = name;
			ReadTime = DateTimeOffset.UtcNow;
			Content = content;
			IsEmbedded = isEmbedded;
		}
	}
}