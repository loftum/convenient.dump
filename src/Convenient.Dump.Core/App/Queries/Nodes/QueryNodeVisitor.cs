using System;

namespace Convenient.Dump.Core.App.Queries.Nodes
{
	public abstract class QueryNodeVisitor<T>
	{
		public T Visit(QueryNode node)
		{
			switch (node)
			{
				case ConstantNode c: return VisitConstant(c);
				case BinaryNode b: return VisitBinary(b);
				case UnaryNode u: return VisitUnary(u);
				case NotNode n: return VisitNot(n);
				default:
					throw new InvalidOperationException($"Unknown node {node.GetType()}");
			}
		}

		protected abstract T VisitConstant(ConstantNode node);
		protected abstract T VisitBinary(BinaryNode binary);
		protected abstract T VisitUnary(UnaryNode unary);
		protected abstract T VisitNot(NotNode not);
	}
}