﻿namespace Convenient.Dump.Core.App.Queries.Nodes
{
	public class UnaryNode : QueryNode
	{
		public UnaryNode(QueryNode operand)
		{
			Operand = operand;
		}

		public QueryNode Operand { get; }

		public override string ToString()
		{
			return $"({Operand})";
		}
	}

	public class NotNode : QueryNode
	{
		public NotNode(QueryNode operand)
		{
			Operand = operand;
		}

		public QueryNode Operand { get; }

		public override string ToString()
		{
			return $"!{Operand}";
		}
	}
}