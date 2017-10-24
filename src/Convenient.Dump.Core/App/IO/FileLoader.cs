using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Convenient.Dump.Core.App.IO
{
	internal class FileLoader
	{
		private static readonly Dictionary<string, FileThingy> Files = new Dictionary<string, FileThingy>();
		private static readonly Assembly Assembly = typeof(FileLoader).Assembly;

		public async Task<string> GetAsync(string name)
		{
			if (Files.TryGetValue(name, out var f) && f.IsUpToDate())
			{
				return f.Content;
			}
			var file = await GetFileThingy(name);
			Files[name] = file;
			return file.Content;
		}

		public async Task<string> GetOrDefaultAsync(string name)
		{
			try
			{
				return await GetAsync(name).ConfigureAwait(false);
			}
			catch
			{
				return null;
			}
		}

		private static async Task<FileThingy> GetFileThingy(string name)
		{
			string content;
			var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App", name);
			if (File.Exists(path))
			{
				content = await File.ReadAllTextAsync(path).ConfigureAwait(false);
				return new FileThingy(path, content, false);
			}
			content = await GetEmbedded(name).ConfigureAwait(false);
			return new FileThingy(name, content, true);
		}

		private static Task<string> GetEmbedded(string name)
		{
			var fullName = $"{Assembly.GetName().Name}.App.{name.Replace("/", ".")}";

			using (var stream = Assembly.GetManifestResourceStream(fullName))
			{
				if (stream == null)
				{
					throw new ArgumentException($"Could not read {fullName}");
				}
				using (var reader = new StreamReader(stream))
				{
					return reader.ReadToEndAsync();
				}
			}
		}
	}
}