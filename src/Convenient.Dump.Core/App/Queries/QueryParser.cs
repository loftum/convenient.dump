using System;
using System.Collections.Generic;
using Convenient.Dump.Core.App.Queries.Nodes;

namespace Convenient.Dump.Core.App.Queries
{
	public class QueryParser
	{
		private readonly ISuperEnumerator<QueryToken> _enumerator;

		public QueryNode Result { get; }

		public QueryParser(ISuperEnumerator<QueryToken> enumerator)
		{
			_enumerator = enumerator;
			_enumerator.MoveNext();
			Result = Parse();
		}


		private QueryNode Parse()
		{
			AdvanceWhile(c => c.Current.Type == TokenType.Whitespace, false);
			if (!_enumerator.Moved)
			{
				return new ConstantNode("*");
			}
			
			var stack = new Stack<QueryNode>();

			while (_enumerator.Moved)
			{
				switch (_enumerator.Current)
				{
					case QueryToken s when s.Type == TokenType.String:
						switch (s.Value)
						{
							case "AND":
								Advance();
								stack.Push(new BinaryNode(BinaryOperand.And, stack.Pop(), Parse()));
								break;
							case "OR":
								Advance();
								stack.Push(new BinaryNode(BinaryOperand.Or, stack.Pop(), Parse()));
								break;
							default:
								Advance(false);
								stack.Push(new ConstantNode(s.Value));
								break;
						}
						break;
					case QueryToken s when s.Type == TokenType.Punctuation:
						switch (s.Value)
						{
							case ":":
								stack.Push(new BinaryNode(BinaryOperand.Equals, stack.Pop(), ReadNextString()));
								Advance(false);
							break;
						}
						break;
					case QueryToken s when s.Type == TokenType.Whitespace:
						Advance(false);
						break;
					default:
						throw new QueryParserException(_enumerator.Current.Position, $"Unexpected token {_enumerator.Current}");
				}
			}
			return stack.Pop();
		}

		private QueryNode ReadNextString()
		{
			Advance();
			AdvanceWhile(c => c.Current.Type == TokenType.Whitespace);
			if (_enumerator.Current.Type != TokenType.String)
			{
				throw new QueryParserException(_enumerator.Current.Position, $"Expected string but got {_enumerator.Current}");
			}
			return new ConstantNode(_enumerator.Current.Value);
		}

		private void AdvanceWhile(Func<IEnumerator<QueryToken>, bool> condition, bool throwOnEnd = true)
		{
			while (condition(_enumerator))
			{
				Advance(throwOnEnd);
			}
		}

		private void Advance(bool throwOnEnd = true)
		{
			var last = _enumerator.Current;
			if (!_enumerator.MoveNext() && throwOnEnd)
			{
				throw QueryParserException.UnexpectedEndAt(last);
			}
		}

		public static QueryNode Parse(string query)
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				return new ConstantNode("*");
			}
			using (var lexer = new QueryLexer(new SourceStreamEnumerator(query.ToStream())))
			{
				var parser = new QueryParser(lexer);
				return parser.Result;
			}
		}
	}
}