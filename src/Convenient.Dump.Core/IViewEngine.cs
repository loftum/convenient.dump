using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Convenient.Dump.Core
{
	public interface IViewEngine
	{
		Task WriteAsync(HttpContext context, object result);
	}
}