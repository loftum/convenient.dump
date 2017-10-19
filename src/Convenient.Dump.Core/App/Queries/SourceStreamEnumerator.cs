using System;
using System.Collections;
using System.IO;

namespace Convenient.Dump.Core.App.Queries
{
	public class SourceStreamEnumerator : ISuperEnumerator<char>
	{
		public bool IsDisposed { get; private set; }

		private readonly Stream _stream;
		private StreamReader _reader;

		private int _lineNumber;
		private int _columnNumber;

		public SourcePosition Position => new SourcePosition(_lineNumber, _columnNumber);

		public SourceStreamEnumerator(Stream stream)
		{
			_stream = stream;
			Init();
		}

		public bool Moved { get; private set; }

		public bool MoveNext()
		{
			Moved = DoMoveNext();
			return Moved;
		}

		private bool DoMoveNext()
		{
			if (_reader.EndOfStream)
			{
				return false;
			}
			var read = _reader.Read();
			if (read == -1)
			{
				return false;
			}
			var lineFeed = _lineNumber == 0 || Current == '\n';
			if (lineFeed)
			{
				_lineNumber++;
				_columnNumber = 1;
			}
			else
			{
				_columnNumber++;
			}
			Current = (char)read;
			return true;
		}

		public void Reset()
		{
			_reader.Dispose();
			Init();
		}

		private void Init()
		{
			_lineNumber = 0;
			_columnNumber = 0;
			_stream.Seek(0, SeekOrigin.Begin);
			_reader = new StreamReader(_stream);
		}

		public char Current { get; private set; }

		object IEnumerator.Current => Current;

		public void Dispose()
		{
			if (IsDisposed)
			{
				return;
			}
			IsDisposed = true;
			_reader.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}