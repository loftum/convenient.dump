using Convenient.Dump.Core.App.Queries;
using ConvenientDump.LiteDb;
using LiteDB;
using Xunit;
using Xunit.Abstractions;

namespace Convenient.Dump.Testing.LiteDb
{
	public class QueryVisitorTest
	{
		private readonly ITestOutputHelper _output;

		public QueryVisitorTest(ITestOutputHelper output)
		{
			_output = output;
		}

		[Fact]
		public void UnquotedNumberIsNumber()
		{
			var query = Parse("field:42");
			Assert.Equal("(field = 42)", query.ToString());
		}

		[Fact]
		public void QuotedNumberIsString()
		{
			var query = Parse("field:\"42\"");
			Assert.Equal("(field = \"42\")", query.ToString());
		}

		[Fact]
		public void Not()
		{
			var query = Parse("field!=42");
			Assert.Equal("!((field = 42))", query.ToString());
		}

		private Query Parse(string expression)
		{
			var node = QueryParser.Parse(expression);
			var query = new QueryVisitor().Visit(node);
			_output.WriteLine(query.ToString());
			return query;
		}
	}
}