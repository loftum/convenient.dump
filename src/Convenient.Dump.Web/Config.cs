using Microsoft.Extensions.Configuration;

namespace Convenient.Dump.Web
{
	public class Config
	{
		public static Config Current { get; }

		static Config()
		{
			var settings = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", false)
				.AddJsonFile("appsettings.local.json", true)
				.Build();
			Current = new Config(settings);
		}

		private readonly IConfiguration _settings;
		public bool RequireHttps => bool.TryParse(_settings["RequireHttps"], out var value) && value;

		public Config(IConfiguration settings)
		{
			_settings = settings;
		}
	}
}