using Microsoft.Xrm.Sdk;
using Remotion.Linq.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace SS.Crm.Linq.Proxies
{
    public class ExpressionTreeVisitorProxy : ThrowingExpressionVisitor
    {
        private ThrowingExpressionVisitor Base = null;
        public ExpressionTreeVisitorProxy(IOrganizationService service, Dictionary<string,string> entityTypeAliases)
        {

        }
        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            throw new NotImplementedException();
        }
        public override Expression Visit(Expression expression)
        {
            return Base.Visit(expression);
        }
        protected override Expression VisitUnknownStandardExpression(Expression expression, string visitMethod, Func<Expression, Expression> baseBehavior)
        {
            return base.VisitUnknownStandardExpression(expression, visitMethod, baseBehavior);
        }
        protected override Expression VisitBinary(BinaryExpression expression)
        {
            return base.VisitBinary(expression);
        }

    }
}
