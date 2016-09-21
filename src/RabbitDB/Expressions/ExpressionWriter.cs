// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="ExpressionWriter.cs">
//   
// </copyright>
// <summary>
//   The expression writer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using RabbitDB.Contracts.Expressions;
using RabbitDB.Mapping;

#endregion

namespace RabbitDB.Expressions
{
    /// <summary>
    ///     The expression writer.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    internal class ExpressionWriter<T> : ExpressionVisitor
    {
        #region Fields

        /// <summary>
        ///     The _expression build helper.
        /// </summary>
        private readonly IDbProviderExpressionBuildHelper _expressionBuildHelper;

        /// <summary>
        ///     The _parameter collection.
        /// </summary>
        private readonly ExpressionParameterCollection _parameterCollection;

        /// <summary>
        ///     The sql builder.
        /// </summary>
        private readonly StringBuilder _sqlBuilder = new StringBuilder();

        /// <summary>
        ///     The _table info.
        /// </summary>
        private readonly TableInfo _tableInfo;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExpressionWriter{T}" /> class.
        /// </summary>
        /// <param name="expressionBuildHelper">
        ///     The expression build helper.
        /// </param>
        /// <param name="parameterCollection">
        ///     The parameter collection.
        /// </param>
        internal ExpressionWriter(
            IDbProviderExpressionBuildHelper expressionBuildHelper,
            ExpressionParameterCollection parameterCollection)
        {
            _expressionBuildHelper = expressionBuildHelper;
            _tableInfo = TableInfo<T>.GetTableInfo;
            _parameterCollection = parameterCollection ?? new ExpressionParameterCollection();
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     Gets the parameters.
        /// </summary>
        internal object[] Parameters => _parameterCollection.ToArray();

        #endregion

        #region Public Methods

        /// <summary>
        ///     The write.
        /// </summary>
        /// <param name="criteria">
        ///     The criteria.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string Write(Expression<Func<T, object>> criteria)
        {
            Visit(criteria);
            return _sqlBuilder.ToString();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     The write.
        /// </summary>
        /// <param name="expression">
        ///     The expression.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        internal string Write(Expression<Func<T, bool>> expression)
        {
            Visit(expression);
            return _sqlBuilder.ToString();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     The visit binary.
        /// </summary>
        /// <param name="node">
        ///     The node.
        /// </param>
        /// <returns>
        ///     The <see cref="Expression" />.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            _sqlBuilder.Append("(");
            Visit(node.Left);

            switch (node.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    _sqlBuilder.Append(" AND ");
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    _sqlBuilder.Append(" OR");
                    break;
                case ExpressionType.Equal:

                    // wenn das rechts daneben null ist, dann "is"
                    if (node.Right.IsNullOrEmpty())
                    {
                        _sqlBuilder.Append(" IS ");
                        break;
                    }

                    _sqlBuilder.Append(" = ");
                    break;
                case ExpressionType.NotEqual:

                    // wenn das rechts daneben null ist, dann "is not"
                    if (node.Right.IsNullOrEmpty())
                    {
                        _sqlBuilder.Append(" IS NOT ");
                        break;
                    }

                    _sqlBuilder.Append(" <> ");
                    break;
                case ExpressionType.LessThan:
                    _sqlBuilder.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    _sqlBuilder.Append(" <= ");
                    break;
                case ExpressionType.GreaterThan:
                    _sqlBuilder.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    _sqlBuilder.Append(" >= ");
                    break;
                case ExpressionType.Add:
                    _sqlBuilder.Append(" + ");
                    break;
                case ExpressionType.Subtract:
                    _sqlBuilder.Append(" - ");
                    break;
                case ExpressionType.Multiply:
                    _sqlBuilder.Append(" * ");
                    break;
                case ExpressionType.Divide:
                    _sqlBuilder.Append(" / ");
                    break;
                default:
                    throw new NotSupportedException($"The binary operator '{node.NodeType}' is not supported");
            }

            Visit(node.Right);

            _sqlBuilder.Append(")");

            return node;
        }

        /// <summary>
        ///     The visit constant.
        /// </summary>
        /// <param name="node">
        ///     The node.
        /// </param>
        /// <returns>
        ///     The <see cref="Expression" />.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            IQueryable queryable = node.Value as IQueryable;

            if (queryable == null && node.Value == null)
            {
                _sqlBuilder.Append("NULL");
            }
            else if (queryable == null)
            {
                switch (Type.GetTypeCode(node.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        _sqlBuilder.Append(((bool)node.Value)
                            ? 1
                            : 0);
                        break;
                    case TypeCode.DateTime:
                    case TypeCode.String:
                        _sqlBuilder.AppendFormat("'{0}'", node.Value);
                        break;
                    case TypeCode.Object:
                        throw new NotSupportedException($"The constant for '{node.Value}' is not supported");
                    default:
                        _sqlBuilder.Append(node.Value);
                        break;
                }
            }

            return node;
        }

        /// <summary>
        ///     The visit member.
        /// </summary>
        /// <param name="node">
        ///     The node.
        /// </param>
        /// <returns>
        ///     The <see cref="Expression" />.
        /// </returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.InvolvesParameter())
            {
                HandleParameter(node);
            }
            else
            {
                _sqlBuilder.Append("@" + _parameterCollection.NextIndex);
                _parameterCollection.Add(node.GetValue());
            }

            return node;
        }

        /// <summary>
        ///     The visit method call.
        /// </summary>
        /// <param name="node">
        ///     The node.
        /// </param>
        /// <returns>
        ///     The <see cref="Expression" />.
        /// </returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.InvolvesParameter())
            {
                HandleParameter(node);
            }
            else
            {
                _sqlBuilder.Append("@" + _parameterCollection.NextIndex);
                _parameterCollection.Add(node.GetValue());
            }

            return node;
        }

        /// <summary>
        ///     The visit unary.
        /// </summary>
        /// <param name="node">
        ///     The node.
        /// </param>
        /// <returns>
        ///     The <see cref="Expression" />.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Not:
                    if (node.Operand.BelongsToParameter()
                        && node.Operand.NodeType == ExpressionType.MemberAccess)
                    {
                        MemberExpression operand = node.Operand as MemberExpression;
                        if (operand != null && operand.Type != typeof(bool))
                        {
                            break;
                        }

                        Expression nex = EqualityFromUnary(node);

                        Visit(nex);

                        break;
                    }

                    _sqlBuilder.Append("NOT ");
                    Visit(node.Operand);
                    break;
                case ExpressionType.Convert:
                    Visit(node.Operand);
                    break;
                default:
                    throw new NotSupportedException($"The unary operator '{node.NodeType}' is not supported");
            }

            return node;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     The equality from unary.
        /// </summary>
        /// <param name="node">
        ///     The node.
        /// </param>
        /// <returns>
        ///     The <see cref="Expression" />.
        /// </returns>
        private static Expression EqualityFromUnary(UnaryExpression node)
        {
            return Expression.Equal(node.Operand, Expression.Constant(node.NodeType != ExpressionType.Not));
        }

        /// <summary>
        ///     The handle contains.
        /// </summary>
        /// <param name="meth">
        ///     The meth.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// </exception>
        private void HandleContains(MethodCallExpression meth)
        {
            IList list;
            var pIdx = 1;

            if (meth.Arguments.Count == 1)
            {
                list = meth.Object.GetValue() as IList;
                pIdx = 0;
            }
            else
            {
                list = meth.Arguments[0].GetValue() as IList;
            }

            if (list == null)
            {
                throw new NotSupportedException("Contains must be invoked on a IList (array or List)");
            }

            var param = meth.Arguments[pIdx] as MemberExpression;

            _sqlBuilder.Append(_expressionBuildHelper.EscapeName(param.Member.Name))
                       .AppendFormat(" in (@{0})", _parameterCollection.NextIndex);
            _parameterCollection.Add(list);
        }

        /// <summary>
        ///     The handle parameter.
        /// </summary>
        /// <param name="node">
        ///     The node.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// </exception>
        private void HandleParameter(MethodCallExpression node)
        {
            if (node.HasParameterArgument())
            {
                if (node.Method.Name != "Contains")
                {
                    throw new NotSupportedException();
                }

                HandleContains(node);
                return;
            }

            if (!node.BelongsToParameter())
            {
                return;
            }

            if (node.Object != null && node.Object.Type == typeof(string))
            {
                HandleParamStringFunctions(node);
            }
        }

        /// <summary>
        ///     The handle parameter.
        /// </summary>
        /// <param name="node">
        ///     The node.
        /// </param>
        /// <param name="isSingle">
        ///     The is single.
        /// </param>
        private void HandleParameter(MemberExpression node, bool isSingle = false)
        {
            if (!node.BelongsToParameter())
            {
                return;
            }

            if (!isSingle)
            {
                if (node.Expression.NodeType == ExpressionType.Parameter)
                {
                    var propertyInfo =
                        _tableInfo.Columns.FirstOrDefault(column => column.Name == node.Member.Name);
                    if (propertyInfo != null)
                    {
                        _sqlBuilder.Append(
                            _expressionBuildHelper.EscapeName(propertyInfo.ColumnAttribute.ColumnName));
                    }
                }
                else
                {
                    HandleParameterSubProperty(node);
                }

                return;
            }

            if (node.Type != typeof(bool))
            {
                return;
            }

            var equalExpression = Expression.Equal(node, Expression.Constant(true));
            Visit(equalExpression);
        }

        /// <summary>
        ///     The handle parameter sub property.
        /// </summary>
        /// <param name="node">
        ///     The node.
        /// </param>
        private void HandleParameterSubProperty(MemberExpression node)
        {
            if (node.Expression.Type == typeof(string))
            {
                HandleStringProperties(node);
            }
        }

        /// <summary>
        ///     The handle param string functions.
        /// </summary>
        /// <param name="node">
        ///     The node.
        /// </param>
        private void HandleParamStringFunctions(MethodCallExpression node)
        {
            var memberExpression = (MemberExpression)node.Object;
            if (memberExpression == null)
            {
                return;
            }

            var memberName = memberExpression.Member.Name;
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
                    value = _expressionBuildHelper.Substring(
                        memberName,
                        (int)arg,
                        (int)node.Arguments[1].GetValue());
                    break;
            }

            _sqlBuilder.Append(value);
        }

        /// <summary>
        ///     The handle string properties.
        /// </summary>
        /// <param name="node">
        ///     The node.
        /// </param>
        private void HandleStringProperties(MemberExpression node)
        {
            var name = ((MemberExpression)node.Expression).Member.Name;
            switch (node.Member.Name)
            {
                case "Length":
                    _sqlBuilder.Append(_expressionBuildHelper.Length(name));
                    break;
            }
        }

        /// <summary>
        ///     The strip quotes.
        /// </summary>
        /// <param name="expression">
        ///     The expression.
        /// </param>
        /// <returns>
        ///     The <see cref="Expression" />.
        /// </returns>
        private static Expression StripQuotes(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Quote)
            {
                expression = ((UnaryExpression)expression).Operand;
            }

            return expression;
        }

        #endregion
    }
}