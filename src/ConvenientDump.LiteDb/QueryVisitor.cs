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
				case BinaryOperand.Eq:
					var left = Visit(binary.Left).Field.Decode()?.ToString();
					var right = Visit(binary.Right).Field.Decode();
					if (right is string s && s.EndsWith("*"))
					{
						return Query.StartsWith(left, s.TrimEnd('*'));
					}
					return Query.EQ(left, new BsonValue(right));
				case BinaryOperand.Lt: return Query.LT(Visit(binary.Left).Field.Decode()?.ToString(), new BsonValue(Visit(binary.Right).Field.Decode()));
				case BinaryOperand.Lte: return Query.LTE(Visit(binary.Left).Field.Decode()?.ToString(), new BsonValue(Visit(binary.Right).Field.Decode()));
				case BinaryOperand.Gt: return Query.GT(Visit(binary.Left).Field.Decode()?.ToString(), new BsonValue(Visit(binary.Right).Field.Decode()));
				case BinaryOperand.Gte: return Query.GTE(Visit(binary.Left).Field.Decode()?.ToString(), new BsonValue(Visit(binary.Right).Field.Decode()));
			}
			throw new InvalidOperationException($"Unknown node {binary}");
		}
	}
}