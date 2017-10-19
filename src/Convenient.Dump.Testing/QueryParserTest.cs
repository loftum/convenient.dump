using Convenient.Dump.Core.App.Queries;
using Convenient.Dump.Core.App.Queries.Nodes;
using Xunit;
using Xunit.Abstractions;

namespace Convenient.Dump.Testing
{
	public class QueryParserTest
	{
		private readonly ITestOutputHelper _output;

		public QueryParserTest(ITestOutputHelper output)
		{
			_output = output;
		}

		[Fact]
		public void ParseEmpty_ReturnsQueryAll()
		{
			var node = (ConstantNode) QueryParser.Parse("");
			Assert.Equal("*", node.Value);
		}

		[Fact]
		public void ParseWhitespace_ReturnsQueryAll()
		{
			var node = (ConstantNode)QueryParser.Parse("  ");
			Assert.Equal("*", node.Value);
		}

		[Fact]
		public void ParseAsterisk_ReturnsQueryAll()
		{
			var node = (ConstantNode) QueryParser.Parse("*");

			Assert.Equal("*", node.Value);
		}

		[Fact]
		public void ParseEquals()
		{
			var node = QueryParser.Parse("field:value");
			_output.WriteLine(node.ToString());
			var binary = (BinaryNode)node;

			Assert.Equal(BinaryOperand.Equals, binary.Op);
		}

		[Fact]
		public void ParseAnd()
		{
			var node = QueryParser.Parse("field:value AND field2:value2");
			_output.WriteLine(node.ToString());
			var binary = (BinaryNode) node;

			Assert.Equal(BinaryOperand.And, binary.Op);
		}

		[Fact]
		public void ParseOr()
		{
			var node = QueryParser.Parse("field:value OR field2:value2");
			_output.WriteLine(node.ToString());
			var binary = (BinaryNode)node;

			Assert.Equal(BinaryOperand.Or, binary.Op);
		}
	}
}