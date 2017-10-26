using System.Threading.Tasks;
using Convenient.Dump.Core.App;

namespace Convenient.Dump.Core
{
	public interface IViewEngine
	{
		Task<string> Render(SimpleContext context, object result);
	}
}