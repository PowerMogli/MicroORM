using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RabbitDB.Expressions
{
    internal static class ExpressionExtensions
    {
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

        internal static bool IsNullOrEmpty(this Expression expression)
        {
            ConstantExpression constantExression = expression as ConstantExpression;
            if (constantExression != null)
                return constantExression.Value == null;

            UnaryExpression unaryExpression = expression as UnaryExpression;
            if (unaryExpression == null) return false;

            constantExression = unaryExpression.Operand as ConstantExpression;
            if (constantExression != null)
                return constantExression.Value == null;

            return false;
        }

        public static bool InvolvesParameter(this Expression node)
        {
            if (node.IsParameter() || node.BelongsToParameter()) return true;
            if (node.NodeType == ExpressionType.Call)
            {
                MethodCallExpression methodCallExpression = node as MethodCallExpression;
                return methodCallExpression.HasParameterArgument();
            }
            return false;
        }

        internal static bool BelongsToParameter(this Expression node)
        {
            Expression parent = null;

            switch (node.NodeType)
            {
                case ExpressionType.MemberAccess:
                    parent = ((MemberExpression)node).Expression;
                    break;
                case ExpressionType.Call:
                    MethodCallExpression methodCallExpression = (MethodCallExpression)node;
                    parent = methodCallExpression.Object;
                    if (methodCallExpression.HasParameterArgument()) return true;
                    break;
                case ExpressionType.Not:
                case ExpressionType.Convert:
                    UnaryExpression unaryExpression = node as UnaryExpression;
                    parent = unaryExpression.Operand;
                    break;
            }

            if (parent == null) return false;

            if (parent.NodeType != ExpressionType.Parameter)
            {
                return parent.BelongsToParameter();
            }
            return true;
        }

        internal static bool HasParameterArgument(this MethodCallExpression node)
        {
            foreach (var arg in node.Arguments)
            {
                if (arg.IsParameter() || arg.BelongsToParameter()) return true;
            }
            return false;
        }

        internal static bool IsParameter(this Expression ex)
        {
            return ex.NodeType == ExpressionType.Parameter;
        }

        internal static object GetValue(this MethodCallExpression node)
        {
            if (node == null) return null;

            if (node.Arguments.Any(a => !a.CanReturnValue()))
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

        internal static object GetValue(this Expression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Constant:
                    ConstantExpression constantExpression = node as ConstantExpression;
                    return constantExpression.Value;
                case ExpressionType.MemberAccess:
                    MemberExpression memberExpression = node as MemberExpression;
                    return memberExpression.GetValue();
                case ExpressionType.Call:
                    MethodCallExpression methodCallExpression = node as MethodCallExpression;
                    return methodCallExpression.GetValue();
            }
            throw new InvalidOperationException("You can get the value of a property,field,constant or method call");
        }

        internal static object GetValue(this MemberExpression memberExpression)
        {
            object parentValue=null;

            if (memberExpression.Expression != null) //not static
            {
                if (memberExpression.Expression.CanReturnValue())
                {
                    parentValue = memberExpression.Expression.GetValue();
                }
            }

            if (memberExpression.Member.MemberType == MemberTypes.Property)
            {
                PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
                return propertyInfo.GetValue(parentValue, null);
            }

            if (memberExpression.Member.MemberType == MemberTypes.Field)
            {
                FieldInfo fieldInfo = memberExpression.Member as FieldInfo;
                return fieldInfo.GetValue(parentValue);
            }

            throw new InvalidOperationException();
        }

        internal static bool CanReturnValue(this Expression node)
        {
            if (node == null) return false;
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
    }
}
