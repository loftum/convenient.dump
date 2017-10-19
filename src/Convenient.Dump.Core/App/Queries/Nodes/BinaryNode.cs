namespace Convenient.Dump.Core.App.Queries.Nodes
{
	public class BinaryNode : QueryNode
	{
		public BinaryOperand Op { get; }
		public QueryNode Left { get; }
		public QueryNode Right { get; }

		public BinaryNode(BinaryOperand op, QueryNode left, QueryNode right)
		{
			Op = op;
			Left = left;
			Right = right;
		}

		public override string ToString()
		{
			return $"{Left} {Op} {Right}";
		}
	}
}