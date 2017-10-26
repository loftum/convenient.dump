using System;
using Convenient.Dump.Core.App.Queries.Nodes;
using LiteDB;

namespace ConvenientDump.LiteDb
{
	public class QueryVisitor : QueryNodeVisitor<Query>
	{
		protected override Query VisitConstant(ConstantNode constant)
		{
			return Query.All(constant.Value.Encode());
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
				case BinaryOperand.Eq: return Query.EQ(Visit(binary.Left).Field.Decode()?.ToString(), new BsonValue(Visit(binary.Right).Field.Decode()));
				case BinaryOperand.Lt: return Query.LT(Visit(binary.Left).Field.Decode()?.ToString(), new BsonValue(Visit(binary.Right).Field.Decode()));
				case BinaryOperand.Lte: return Query.LTE(Visit(binary.Left).Field.Decode()?.ToString(), new BsonValue(Visit(binary.Right).Field.Decode()));
				case BinaryOperand.Gt: return Query.GT(Visit(binary.Left).Field.Decode()?.ToString(), new BsonValue(Visit(binary.Right).Field.Decode()));
				case BinaryOperand.Gte: return Query.GTE(Visit(binary.Left).Field.Decode()?.ToString(), new BsonValue(Visit(binary.Right).Field.Decode()));
			}
			throw new InvalidOperationException($"Unknown node {binary}");
		}
	}
}