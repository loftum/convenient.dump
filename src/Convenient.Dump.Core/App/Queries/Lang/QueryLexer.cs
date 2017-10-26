using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Convenient.Dump.Core.App.Queries.Lang
{
	public class QueryLexer : ISuperEnumerator<QueryToken>
	{
		public bool IsDisposed { get; private set; }
		public QueryToken Current { get; private set; }
		object IEnumerator.Current => Current;
		public bool Moved { get; private set; }

		private readonly SourceStreamEnumerator _enumerator;

		public QueryLexer(SourceStreamEnumerator enumerator)
		{
			_enumerator = enumerator;
			_enumerator.MoveNext();
		}

		public bool MoveNext()
		{
			Moved = DoMoveNext();
			return Moved;
		}


		private bool DoMoveNext()
		{
			while (_enumerator.Moved)
			{
				var current = _enumerator.Current;

				switch (current)
				{
					case '"':
						Current = ReadQuotedString();
						return true;
					case '*':
					case var c when c.IsLetterOrDigit():
						Current = ReadAlphanumericOr('*');
						return true;
					case var c when c.IsWhitespace():
						Current = ReadWhitespace();
						return true;
					case var c when c.IsPunctuation():
						Current = Read(d => d.IsPunctuation() && d!='"' && d != '\\', TokenType.Punctuation);
						return true;
					case var c when c.IsSymbol():
						Current = Read(d => d.IsSymbol(), TokenType.Symbol);
						return true;
				}
				return true;
			}
			return false;
		}

		private QueryToken ReadQuotedString()
		{
			var value = new StringBuilder();
			var position = _enumerator.Position;
			while (_enumerator.MoveNext() && _enumerator.Current != '"')
			{
				value.Append(_enumerator.Current);
			}
			_enumerator.MoveNext();
			return new QueryToken(TokenType.String, position, value.ToString());
		}

		private QueryToken Read(Func<char, bool> condition, TokenType type)
		{
			var value = new StringBuilder();
			var position = _enumerator.Position;
			while (condition(_enumerator.Current))
			{
				value.Append(_enumerator.Current);
				if (!_enumerator.MoveNext())
				{
					break;
				}
			}
			return new QueryToken(type, position, value.ToString());
		}

		private QueryToken ReadCharAs(TokenType type)
		{
			var token = new QueryToken(type, _enumerator.Position, _enumerator.Current.ToString());
			_enumerator.MoveNext();
			return token;
		}

		private QueryToken ReadWhitespace()
		{
			var value = new StringBuilder();
			var position = _enumerator.Position;
			while (_enumerator.Current.IsWhitespace())
			{
				value.Append((char) _enumerator.Current);
				if (!_enumerator.MoveNext())
				{
					break;
				}
			}
			return new QueryToken(TokenType.Whitespace, position, value.ToString());
		}

		private QueryToken ReadAlphanumericOr(char other, params char[] others)
		{
			var allOthers = new[] {other}.Concat(others).ToList();
			var value = new StringBuilder();
			var position = _enumerator.Position;
			var allNumbers = true;
			while (_enumerator.Current.IsLetterOrDigit() || allOthers.Contains(_enumerator.Current))
			{
				value.Append(_enumerator.Current);
				if (!_enumerator.Current.IsNumber())
				{
					allNumbers = false;
				}
				if (!_enumerator.MoveNext())
				{
					break;
				}
			}
			var raw = value.ToString();
			return allNumbers
				? new QueryToken(TokenType.Number, position, raw, int.Parse(raw))
				: new QueryToken(TokenType.String, position, value.ToString());
		}

		public void Reset()
		{
			_enumerator.Reset();
			Current = default(QueryToken);
		}

		public void Dispose()
		{
			if (IsDisposed)
			{
				return;
			}
			IsDisposed = true;
			_enumerator.Dispose();
		}
	}
}