using System;
using Convenient.Dump.Core.App.Queries.Nodes;
using LiteDB;

namespace ConvenientDump.LiteDb
{
	public class QueryVisitor : QueryNodeVisitor<Query>
	{
		protected override Query VisitConstant(ConstantNode constant)
		{
			return Query.All(constant.Value);
		}

		protected override Query VisitUnary(UnaryNode unary)
		{
			return Visit(unary.Operand);
		}

		protected override Query VisitBinary(BinaryNode binary)
		{
			switch (binary.Op)
			{
				case BinaryOperand.And: return Query.And(Visit(binary.Left), Visit(binary.Right));
				case BinaryOperand.Or: return Query.Or(Visit(binary.Left), Visit(binary.Right));
				case BinaryOperand.Equals:
					return Query.EQ(Visit(binary.Left).Field, Visit(binary.Right).Field);
			}
			throw new InvalidOperationException($"Unknown node {binary.GetType()}");
		}
	}
}