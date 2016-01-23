﻿namespace Crayon.ParseTree
{
	internal class NullConstant : Expression
	{
		public NullConstant(Token token)
			: base(token)
		{ }

		public override bool IsLiteral { get { return true; } }

		internal override Expression Resolve(Parser parser)
		{
			return this;
		}

		internal override void VariableUsagePass(Parser parser)
		{
		}

		internal override void VariableIdAssignmentPass(Parser parser)
		{
		}
	}
}