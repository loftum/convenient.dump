namespace Convenient.Dump.Core.App.Queries.Nodes
{
	public class ConstantNode : QueryNode
	{
		public object Value { get; }

		public ConstantNode(object value)
		{
			Value = value;
		}

		public override string ToString()
		{
			return Value?.ToString();
		}
	}
}