using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Convenient.Dump.Core
{
	public interface IViewEngine
	{
		Task<string> Render(HttpContext context, object result);
	}
}