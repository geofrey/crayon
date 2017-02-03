﻿using System;
using System.Collections.Generic;

namespace Pastel.Nodes
{
    class ForcedParenthesis : Expression
    {
        public Expression Expression { get; set; }

        public ForcedParenthesis(Token token, Expression expression) : base(token)
        {
            this.Expression = expression;
            this.ResolvedType = expression.ResolvedType;
        }

        public override Expression ResolveNamesAndCullUnusedCode(PastelCompiler compiler)
        {
            throw new NotImplementedException();
        }

        internal override Expression ResolveType(VariableScope varScope, PastelCompiler compiler)
        {
            throw new NotImplementedException();
        }
    }
}
