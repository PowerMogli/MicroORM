// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="ExpressionExtensions.cs">
//   
// </copyright>
// <summary>
//   The expression extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Expressions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// The expression extensions.
    /// </summary>
    internal static class ExpressionExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get property name.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        public static string GetPropertyName(this Expression node)
        {
            var member = node as MemberExpression;
            if (member == null)
            {
                var unary = node as UnaryExpression;
                if (unary == null)
                {
                    throw new ArgumentException("Expression must be MemberExpression or UnaryExpression");
                }

                member = unary.Operand as MemberExpression;
            }

            if (member == null)
            {
                throw new ArgumentException("Expression isn't a member access");
            }

            return member.Member.Name;
        }

        /// <summary>
        /// The involves parameter.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool InvolvesParameter(this Expression node)
        {
            if (node.IsParameter() || node.BelongsToParameter())
            {
                return true;
            }

            if (node.NodeType == ExpressionType.Call)
            {
                var methodCallExpression = node as MethodCallExpression;
                return methodCallExpression.HasParameterArgument();
            }

            return false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The belongs to parameter.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal static bool BelongsToParameter(this Expression node)
        {
            Expression parent = null;

            switch (node.NodeType)
            {
                case ExpressionType.MemberAccess:
                    parent = ((MemberExpression)node).Expression;
                    break;
                case ExpressionType.Call:
                    var methodCallExpression = (MethodCallExpression)node;
                    parent = methodCallExpression.Object;
                    if (methodCallExpression.HasParameterArgument())
                    {
                        return true;
                    }

                    break;
                case ExpressionType.Not:
                case ExpressionType.Convert:
                    var unaryExpression = node as UnaryExpression;
                    parent = unaryExpression.Operand;
                    break;
            }

            if (parent == null)
            {
                return false;
            }

            if (parent.NodeType != ExpressionType.Parameter)
            {
                return parent.BelongsToParameter();
            }

            return true;
        }

        /// <summary>
        /// The can return value.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal static bool CanReturnValue(this Expression node)
        {
            if (node == null)
            {
                return false;
            }

            switch (node.NodeType)
            {
                case ExpressionType.Constant:
                case ExpressionType.New:
                case ExpressionType.NewArrayInit:
                case ExpressionType.MemberAccess:
                case ExpressionType.Call:
                case ExpressionType.ArrayIndex:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// The get value.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        internal static object GetValue(this MethodCallExpression node)
        {
            if (node == null)
            {
                return null;
            }

            if (node.Arguments.Any(a => !CanReturnValue(a)))
            {
                throw new NotSupportedException("Can't identify the value of at least one argument");
            }

            var args = node.Arguments.Select(a => a.GetValue()).ToArray();
            object parent = null;

            if (node.Object != null && node.Object.CanReturnValue())
            {
                parent = node.Object.GetValue();
            }

            return node.Method.Invoke(parent, args);
        }

        /// <summary>
        /// The get value.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        internal static object GetValue(this Expression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Constant:
                    var constantExpression = node as ConstantExpression;
                    if (constantExpression != null)
                    {
                        return constantExpression.Value;
                    }

                    break;
                case ExpressionType.MemberAccess:
                    var memberExpression = node as MemberExpression;
                    return memberExpression.GetValue();
                case ExpressionType.Call:
                    var methodCallExpression = node as MethodCallExpression;
                    return methodCallExpression.GetValue();
            }

            throw new InvalidOperationException("You can get the value of a property,field,constant or method call");
        }

        /// <summary>
        /// The get value.
        /// </summary>
        /// <param name="memberExpression">
        /// The member expression.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        internal static object GetValue(this MemberExpression memberExpression)
        {
            object parentValue = null;

            if (memberExpression.Expression != null)
            {
                // not static
                if (memberExpression.Expression.CanReturnValue())
                {
                    parentValue = memberExpression.Expression.GetValue();
                }
            }

            if (memberExpression.Member.MemberType == MemberTypes.Property)
            {
                var propertyInfo = memberExpression.Member as PropertyInfo;
                if (propertyInfo != null)
                {
                    return propertyInfo.GetValue(parentValue, null);
                }
            }

            if (memberExpression.Member.MemberType != MemberTypes.Field)
            {
                throw new InvalidOperationException();
            }

            var fieldInfo = memberExpression.Member as FieldInfo;
            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(parentValue);
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// The has parameter argument.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal static bool HasParameterArgument(this MethodCallExpression node)
        {
            return node.Arguments.Any(arg => arg.IsParameter() || arg.BelongsToParameter());
        }

        /// <summary>
        /// The is null or empty.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal static bool IsNullOrEmpty(this Expression expression)
        {
            var constantExression = expression as ConstantExpression;
            if (constantExression != null)
            {
                return constantExression.Value == null;
            }

            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression == null)
            {
                return false;
            }

            constantExression = unaryExpression.Operand as ConstantExpression;
            if (constantExression != null)
            {
                return constantExression.Value == null;
            }

            return false;
        }

        /// <summary>
        /// The is parameter.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal static bool IsParameter(this Expression ex)
        {
            return ex.NodeType == ExpressionType.Parameter;
        }

        #endregion
    }
}