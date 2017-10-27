using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Convenient.Dump.Core.App.Queries.Lang;
using Convenient.Dump.Core.App.Queries.Nodes;

namespace Convenient.Dump.Core.App.Queries
{
	public class QueryParser
	{
		private readonly Stack<QueryNode> _stack = new Stack<QueryNode>();

		private static readonly Dictionary<string, BinaryOperand> OperandMap = new Dictionary<string, BinaryOperand>
		{
			["="] = BinaryOperand.Eq,
			["=="] = BinaryOperand.Eq,
			[":"] = BinaryOperand.Eq,
			["<"] = BinaryOperand.Lt,
			["<="] = BinaryOperand.Lte,
			["<:"] = BinaryOperand.Lte,
			[">"] = BinaryOperand.Gt,
			[">="] = BinaryOperand.Gte,
			[">:"] = BinaryOperand.Gte
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
			return _enumerator.Moved ?
				ParseWhile(() => _enumerator.Moved)
				: new ConstantNode("*");
		}

		private QueryNode ParseWhile(Func<bool> condition)
		{
			while (_enumerator.Moved && condition())
			{
				var current = _enumerator.Current;
				switch (current.StringValue)
				{
					case "AND":
					case "&&":
						Advance();
						_stack.Push(new BinaryNode(BinaryOperand.And, _stack.Pop(), ParseWhile(condition)));
						continue;
					case "OR":
					case "||":
						Advance();
						_stack.Push(new BinaryNode(BinaryOperand.Or, _stack.Pop(), ParseWhile(condition)));
						continue;
				}
				
				switch (current.Type)
				{
					case TokenType.Number:
						Advance(false);
						_stack.Push(new ConstantNode(current.Value));
						break;
					case TokenType.String:
						switch (current.Value)
						{
							default:
								_stack.Push(ReadField());
								break;
						}
						break;
					case TokenType.Symbol:
					case TokenType.Punctuation:
						switch (current.Value)
						{
							case "!":
								Advance();
								_stack.Push(new NotNode(ParseWhile(condition)));
								break;
							case "(":
								_stack.Push(ParseUnary());
								break;
							case string v when OperandMap.TryGetValue(v, out var operand):
								if (!_stack.Any())
								{
									throw new QueryParserException(_enumerator.Current.Position, $"Unexpected token {_enumerator.Current}");
								}
								var left = _stack.Pop();
								var right = ReadNextThing();
								_stack.Push(new BinaryNode(operand, left, right));
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
			return _stack.Pop();
		}

		private QueryNode ParseUnary()
		{
			Advance();
			var operand = ParseWhile(() => _enumerator.Current.StringValue != ")");
			Advance(false);
			return new UnaryNode(operand);
		}

		private QueryNode ReadField()
		{
			var value = new StringBuilder();
			while (_enumerator.Moved && (_enumerator.Current.Type == TokenType.String || _enumerator.Current.StringValue == "."))
			{
				value.Append(_enumerator.Current.Value);
				if (!_enumerator.MoveNext())
				{
					break;
				}
			}
			return new ConstantNode(value.ToString());
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