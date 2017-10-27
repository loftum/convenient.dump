using Convenient.Dump.Core.App.Queries;
using Convenient.Dump.Core.App.Queries.Lang;
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
			var node = Parse<ConstantNode>("");
			Assert.Equal("*", node.Value);
		}

		[Fact]
		public void ParseWhitespace_ReturnsQueryAll()
		{
			var node = Parse<ConstantNode>("  ");
			Assert.Equal("*", node.Value);
		}

		[Fact]
		public void ParseAsterisk_ReturnsQueryAll()
		{
			var node = Parse<ConstantNode>("*");
			Assert.Equal("*", node.Value);
		}

		[Fact]
		public void ParseEqualsWithQuotes()
		{
			var node = Parse<BinaryNode>("\"_field\":value");
			Assert.Equal(BinaryOperand.Eq, node.Op);
		}

		[Fact]
		public void ParseEquals()
		{
			var node = Parse<BinaryNode>("field:value");
			Assert.Equal(BinaryOperand.Eq, node.Op);
		}

		[Fact]
		public void ParseNestedField()
		{
			var node = Parse<BinaryNode>("field.nestedField==value");
			Assert.Equal(BinaryOperand.Eq, node.Op);
		}

		[Fact]
		public void ParseAnd()
		{
			var node = Parse<BinaryNode>("field:value AND field2:value2");
			Assert.Equal(BinaryOperand.And, node.Op);
		}

		[Fact]
		public void ParseOr()
		{
			var node = Parse<BinaryNode>("field:value OR field2:value2");
			Assert.Equal(BinaryOperand.Or, node.Op);
		}

		[Fact]
		public void ParseUnary()
		{
			var node = Parse<UnaryNode>("(field:value)");
			Assert.IsAssignableFrom<BinaryNode>(node.Operand);
		}

		[Fact]
		public void UnaryError()
		{
			Assert.Throws<QueryParserException>(() => QueryParser.Parse("("));
		}

		[Fact]
		public void ParseNumber()
		{
			var node = Parse<BinaryNode>("field:42");
			var right = (ConstantNode) node.Right;
			Assert.IsType<int>(right.Value);
		}

		[Fact]
		public void ParseNot()
		{
			Parse<NotNode>("field!=42");
		}

		private QueryNode Parse(string query)
		{
			var node = QueryParser.Parse(query);
			_output.WriteLine($"Query: {node}");
			_output.WriteLine("");
			_output.WriteLine("Syntax tree:");
			_output.WriteLine(Json.Serialize(node, true));
			return node;
		}

		private T Parse<T>(string query) where T : QueryNode
		{
			var node = Parse(query);
			Assert.IsAssignableFrom<T>(node);
			return (T) node;
		}
	}
}