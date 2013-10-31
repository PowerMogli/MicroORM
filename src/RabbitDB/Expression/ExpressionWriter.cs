using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using RabbitDB.Mapping;

namespace RabbitDB.Expressions
{
    internal class ExpressionWriter<T> : ExpressionVisitor
    {
        private StringBuilder sqlBuilder = new StringBuilder();
        private IDbProviderExpressionBuildHelper _expressionBuildHelper;
        private TableInfo _tableInfo;
        private ExpressionParameterCollection _parameterCollection;

        internal object[] Parameters
        { get { return _parameterCollection.ToArray(); } }

        internal ExpressionWriter(IDbProviderExpressionBuildHelper expressionBuildHelper, ExpressionParameterCollection parameterCollection)
        {
            _expressionBuildHelper = expressionBuildHelper;
            _tableInfo = TableInfo<T>.GetTableInfo;
            _parameterCollection = parameterCollection ?? new ExpressionParameterCollection();
        }

        internal string Write(Expression<Func<T, bool>> expression)
        {
            Visit(expression);
            return sqlBuilder.ToString();
        }

        public string Write<T>(Expression<Func<T, object>> criteria)
        {
            Visit(criteria);
            return sqlBuilder.ToString();
        }

        private static Expression StripQuotes(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Quote)
            {
                expression = ((UnaryExpression)expression).Operand;
            }
            return expression;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.InvolvesParameter())
            {
                HandleParameter(node);
            }
            else
            {
                sqlBuilder.Append("@" + _parameterCollection.NextIndex);
                _parameterCollection.Add(node.GetValue());
            }
            return node;

        }

        private void HandleParameter(MethodCallExpression node)
        {
            if (node.HasParameterArgument())
            {
                if (node.Method.Name == "Contains")
                {
                    HandleContains(node);
                    return;
                }
                throw new NotSupportedException();
            }

            if (node.BelongsToParameter())
            {
                if (node.Object.Type == typeof(string))
                {
                    HandleParamStringFunctions(node);
                }
            }
        }

        private void HandleParamStringFunctions(MethodCallExpression node)
        {
            string memberName = ((MemberExpression)node.Object).Member.Name;
            var arg = node.Arguments[0].GetValue();
            string value = null;
            switch (node.Method.Name)
            {
                case "StartsWith":
                    value = string.Format("{0} like '{1}%'", _expressionBuildHelper.EscapeName(memberName), arg);
                    break;
                case "EndsWith":
                    value = string.Format("{0} like '%{1}'", _expressionBuildHelper.EscapeName(memberName), arg);
                    break;
                case "Contains":
                    value = string.Format("{0} like '%{1}%'", _expressionBuildHelper.EscapeName(memberName), arg);
                    break;
                case "ToUpper":
                case "ToUpperInvariant":
                    value = _expressionBuildHelper.ToUpper(memberName);
                    break;
                case "ToLower":
                case "ToLowerInvariant":
                    value = _expressionBuildHelper.ToLower(memberName);
                    break;
                case "Substring":
                    value = _expressionBuildHelper.Substring(memberName, (int)arg, (int)node.Arguments[1].GetValue());
                    break;
            }

            sqlBuilder.Append(value);
        }

        private void HandleContains(MethodCallExpression meth)
        {
            IList list = null;
            int pIdx = 1;
            if (meth.Arguments.Count == 1)
            {
                list = meth.Object.GetValue() as IList;
                pIdx = 0;
            }
            else
                list = meth.Arguments[0].GetValue() as IList;

            if (list == null)
                throw new NotSupportedException("Contains must be invoked on a IList (array or List)");

            var param = meth.Arguments[pIdx] as MemberExpression;

            sqlBuilder.Append(_expressionBuildHelper.EscapeName(param.Member.Name)).AppendFormat(" in (@{0})", _parameterCollection.NextIndex);
            _parameterCollection.Add(list);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Not:
                    if (node.Operand.BelongsToParameter()
                        && node.Operand.NodeType == ExpressionType.MemberAccess)
                    {
                        var oper = node.Operand as MemberExpression;
                        if (oper.Type != typeof(bool)) break;

                        var nex = EqualityFromUnary(node);
                        Visit(nex);
                        break;
                    }

                    sqlBuilder.Append("NOT ");
                    this.Visit(node.Operand);
                    break;
                case ExpressionType.Convert:
                    this.Visit(node.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", node.NodeType));
            }
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            IQueryable q = node.Value as IQueryable;

            if (q == null && node.Value == null)
            {
                sqlBuilder.Append("NULL");
            }
            else if (q == null)
            {
                switch (Type.GetTypeCode(node.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        sqlBuilder.Append(((bool)node.Value) ? 1 : 0);
                        break;
                    case TypeCode.DateTime:
                    case TypeCode.String:
                        sqlBuilder.AppendFormat("'{0}'", node.Value);
                        break;
                    case TypeCode.Object:
                        throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", node.Value));
                    default:
                        sqlBuilder.Append(node.Value);
                        break;
                }
            }

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.InvolvesParameter())
            {
                HandleParameter(node);
            }
            else
            {
                sqlBuilder.Append("@" + _parameterCollection.NextIndex);
                _parameterCollection.Add(node.GetValue());
            }

            return node;
        }

        private void HandleParameter(MemberExpression node, bool isSingle = false)
        {
            if (node.BelongsToParameter())
            {
                if (!isSingle)
                {
                    if (node.Expression.NodeType == ExpressionType.Parameter)
                    {
                        IPropertyInfo propertyInfo = _tableInfo.Columns.FirstOrDefault(column => column.Name == node.Member.Name);
                        sqlBuilder.Append(_expressionBuildHelper.EscapeName(propertyInfo.ColumnAttribute.ColumnName));
                    }
                    else
                    {
                        HandleParameterSubProperty(node);
                    }
                    return;
                }
                if (node.Type == typeof(bool))
                {
                    var equalExpression = Expression.Equal(node, Expression.Constant(true));
                    Visit(equalExpression);
                    return;
                }
            }
        }

        private void HandleParameterSubProperty(MemberExpression node)
        {
            if (node.Expression.Type == typeof(string))
            {
                HandleStringProperties(node);
                return;
            }
        }

        private void HandleStringProperties(MemberExpression node)
        {
            string name = ((MemberExpression)node.Expression).Member.Name;
            switch (node.Member.Name)
            {
                case "Length":
                    sqlBuilder.Append(_expressionBuildHelper.Length(name));
                    break;
            }
        }

        private Expression EqualityFromUnary(UnaryExpression node)
        {
            return Expression.Equal(node.Operand, Expression.Constant(node.NodeType != ExpressionType.Not));
        }

        //protected bool IsNullConstant(Expression exp)
        //{
        //    return (exp.NodeType == ExpressionType.Constant && ((ConstantExpression)exp).Value == null);
        //}

        //private bool ParseOrderByExpression(MethodCallExpression expression, string order)
        //{
        //    UnaryExpression unary = (UnaryExpression)expression.Arguments[1];
        //    LambdaExpression lambdaExpression = (LambdaExpression)unary.Operand;

        //    lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

        //    MemberExpression body = lambdaExpression.Body as MemberExpression;
        //    if (body != null)
        //    {
        //        if (string.IsNullOrEmpty(_orderBy))
        //        {
        //            _orderBy = string.Format("{0} {1}", body.Member.Name, order);
        //        }
        //        else
        //        {
        //            _orderBy = string.Format("{0}, {1} {2}", _orderBy, body.Member.Name, order);
        //        }

        //        return true;
        //    }

        //    return false;
        //}

        //private bool ParseTakeExpression(MethodCallExpression expression)
        //{
        //    ConstantExpression sizeExpression = (ConstantExpression)expression.Arguments[1];

        //    int size;
        //    if (int.TryParse(sizeExpression.Value.ToString(), out size))
        //    {
        //        _take = size;
        //        return true;
        //    }

        //    return false;
        //}

        //private bool ParseSkipExpression(MethodCallExpression expression)
        //{
        //    ConstantExpression sizeExpression = (ConstantExpression)expression.Arguments[1];

        //    int size;
        //    if (int.TryParse(sizeExpression.Value.ToString(), out size))
        //    {
        //        _skip = size;
        //        return true;
        //    }

        //    return false;
        //}

        protected override Expression VisitBinary(BinaryExpression node)
        {
            sqlBuilder.Append("(");
            this.Visit(node.Left);

            switch (node.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    sqlBuilder.Append(" AND ");
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    sqlBuilder.Append(" OR");
                    break;
                case ExpressionType.Equal:
                    // wenn das rechts daneben null ist, dann "is"
                    if (node.Right.IsNullOrEmpty())
                    {
                        sqlBuilder.Append(" IS ");
                        break;
                    }
                    sqlBuilder.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    // wenn das rechts daneben null ist, dann "is not"
                    if (node.Right.IsNullOrEmpty())
                    {
                        sqlBuilder.Append(" IS NOT ");
                        break;
                    }
                    sqlBuilder.Append(" <> ");
                    break;
                case ExpressionType.LessThan:
                    sqlBuilder.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    sqlBuilder.Append(" <= ");
                    break;
                case ExpressionType.GreaterThan:
                    sqlBuilder.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    sqlBuilder.Append(" >= ");
                    break;
                case ExpressionType.Add:
                    sqlBuilder.Append(" + ");
                    break;
                case ExpressionType.Subtract:
                    sqlBuilder.Append(" - ");
                    break;
                case ExpressionType.Multiply:
                    sqlBuilder.Append(" * ");
                    break;
                case ExpressionType.Divide:
                    sqlBuilder.Append(" / ");
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", node.NodeType));
            }
            this.Visit(node.Right);
            sqlBuilder.Append(")");
            return node;
        }
    }
}
