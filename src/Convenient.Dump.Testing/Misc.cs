using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Convenient.Dump.Testing
{
	public class Misc
	{
		private readonly ITestOutputHelper _output;

		public Misc(ITestOutputHelper output)
		{
			_output = output;
		}

		[Fact]
		public void CharProperties()
		{
			PrintPropertiesOf('(');
		}

		private void PrintPropertiesOf(char c)
		{
			var methods = from m in typeof(char).GetMethods(BindingFlags.Public | BindingFlags.Static)
				let parameters = m.GetParameters()
				where m.ReturnType == typeof(bool) && parameters.Length == 1 && parameters.Single().ParameterType == typeof(char)
				select m;

			foreach (var method in methods)
			{
				_output.WriteLine($"{method.Name}: {method.Invoke(null, new object[] { c })}");
			}
		}
	}
}