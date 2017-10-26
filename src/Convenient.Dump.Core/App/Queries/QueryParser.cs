using System;
using System.Collections.Generic;
using System.Linq;
using Convenient.Dump.Core.App.Queries.Lang;
using Convenient.Dump.Core.App.Queries.Nodes;

namespace Convenient.Dump.Core.App.Queries
{
	public class QueryParser
	{
		private static readonly Dictionary<string, BinaryOperand> OperandMap = new Dictionary<string, BinaryOperand>
		{
			[":"] = BinaryOperand.Eq,
			["<"] = BinaryOperand.Lt,
			["<="] = BinaryOperand.Lte,
			[">"] = BinaryOperand.Gt,
			[">="] = BinaryOperand.Gte
		};

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
			return ParseWhile(() => _enumerator.Moved);
		}

		private QueryNode ParseWhile(Func<bool> condition)
		{
			var stack = new Stack<QueryNode>();

			while (_enumerator.Moved && condition())
			{
				var current = _enumerator.Current;
				switch (current.Type)
				{
					case TokenType.Number:
						Advance(false);
						stack.Push(new ConstantNode(current.Value));
						break;
					case TokenType.String:
						switch (current.Value)
						{
							case "AND":
								Advance();
								stack.Push(new BinaryNode(BinaryOperand.And, stack.Pop(), ParseWhile(condition)));
								break;
							case "OR":
								Advance();
								stack.Push(new BinaryNode(BinaryOperand.Or, stack.Pop(), ParseWhile(condition)));
								break;
							default:
								Advance(false);
								stack.Push(new ConstantNode(current.Value));
								break;
						}
						break;
					case TokenType.Symbol:
					case TokenType.Punctuation:
						switch (current.Value)
						{
							case "(":
								Advance();
								stack.Push(new UnaryNode(ParseWhile(() => _enumerator.Current.StringValue != ")")));
								Advance(false);
								break;
							case string v when OperandMap.TryGetValue(v, out var operand):
								if (!stack.Any())
								{
									throw new QueryParserException(_enumerator.Current.Position, $"Unexpected token {_enumerator.Current}");
								}
								var left = stack.Pop();
								stack.Push(new BinaryNode(operand, left, ReadNextThing()));
								Advance(false);
								break;
							default:
								throw new QueryParserException(_enumerator.Current.Position, $"Unexpected token {_enumerator.Current}");
						}
						break;
					case TokenType.Whitespace:
						Advance(false);
						break;
					default:
						throw new QueryParserException(_enumerator.Current.Position, $"Unexpected token {_enumerator.Current}");
				}
			}
			return stack.Pop();
		}

		private QueryNode ReadNextThing()
		{
			Advance();
			AdvanceWhile(c => c.Current.Type == TokenType.Whitespace);
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