using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using decaf.common;
using decaf.common.Utilities.Reflection;
using decaf.state.Conditionals;

namespace decaf.state.Utilities
{
    class CallExpressionHelper
    {
        private readonly IExpressionHelper expressionHelper;
        private readonly IValueFunctionHelper valueFunctionHelper;

        public CallExpressionHelper(
            IExpressionHelper expressionHelper,
            IValueFunctionHelper valueFunctionHelper)
        {
            this.expressionHelper = expressionHelper;
            this.valueFunctionHelper = valueFunctionHelper;
        }

        public IWhere ParseExpression(Expression expression, IQueryContextExtended context)
        {
            Expression expressionToParse;
            if (expression is LambdaExpression)
            {
                var lambda = expression as LambdaExpression;
                var unary = lambda.Body as UnaryExpression;
                if (unary?.NodeType == ExpressionType.Not)
                {
                    expressionToParse = Expression.Equal(unary.Operand, Expression.Constant(false));
                }
                else
                {
                    expressionToParse = lambda.Body;
                }
            }
            else
            {
                expressionToParse = expression;
            }

            if(expressionHelper.IsMethodCallOnProperty(expressionToParse))
            {
                var binaryExpression = expressionToParse as BinaryExpression;
                if(binaryExpression == null)
                {
                    var constantExpression = Expression.Constant(true);
                    binaryExpression = Expression.Equal(expressionToParse, constantExpression);
                }
                return ParseBinaryExpression(binaryExpression, context);
            }

            if(expressionHelper.IsMethodCallOnConstantOrMemberAccess(expressionToParse))
            {
                var binaryExpression = expressionToParse as BinaryExpression;
                if (binaryExpression == null)
                {
                    return ParseMethodAccessCall(expressionToParse, context);
                }

                var call = binaryExpression.Left as MethodCallExpression ??
                           binaryExpression.Right as MethodCallExpression;
                var constant = binaryExpression.Left as ConstantExpression ??
                               binaryExpression.Right as ConstantExpression;
                var result = ParseMethodAccessCall(call, context);
                var constantValue = (bool)expressionHelper.GetValue(constant);

                if (binaryExpression.NodeType == ExpressionType.NotEqual ||
                    !constantValue)
                    return Not.This(result);

                return result;
            }

            return ParseBinaryExpression(expressionToParse, context);
        }

        private IWhere ParseBinaryExpression(
            Expression expression,
            IQueryContextExtended context)
        {
            GetCallAndNonCallExpressions(
                expression,
                out var callExpr,
                out var nonCallExpr);

            if (callExpr == null) return null;

            var valueFunction = valueFunctionHelper.ParseFunction(callExpr);
            if (valueFunction == null) return null;

            var memberExpression = callExpr.Object as MemberExpression ??
                                   callExpr.Arguments[0] as MemberExpression;

            var op = expressionHelper.ConvertExpressionTypeToEqualityOperator(expression);
            var fieldName = expressionHelper.GetMemberName(memberExpression);
            var target = context.GetQueryTarget(memberExpression);
            var col = Column.Create(fieldName, target);

            IWhere result = null;
            var invertResult = op == EqualityOperator.NotEquals;

            if (nonCallExpr.NodeType == ExpressionType.Constant)
            {
                var value = expressionHelper.GetValue(nonCallExpr);

                if(value is bool)
                {
                    var boolValue = (bool)value;
                    if(!boolValue && op == EqualityOperator.Equals)
                    {
                        value = invertResult = true;
                    }
                }

                var functionType = typeof(Column<>);
                var genericType = value?.GetType() ?? memberExpression?.Type;
                var implementedType = functionType.MakeGenericType(genericType);
                var parameters = new object[] { col, op, valueFunction, value };
                var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
                result = (IWhere)Activator.CreateInstance(
                    implementedType,
                    bindingFlags,
                    null,
                    parameters,
                    System.Globalization.CultureInfo.CurrentCulture);
            }

            if (nonCallExpr.NodeType == ExpressionType.MemberAccess)
            {
                var fieldB = expressionHelper.GetMemberName(nonCallExpr);
                var targetB = context.GetQueryTarget(nonCallExpr);
                var colB = Column.Create(fieldB, targetB);
                result = Conditionals.Column.Equals(col, op, valueFunction, colB);
            }

            if (nonCallExpr.NodeType == ExpressionType.Call)
            {
                var rightCallExpr = nonCallExpr as MethodCallExpression;
                var rightBody = rightCallExpr?.Object;
                var fieldB = expressionHelper.GetMemberName(rightBody);
                var targetB = context.GetQueryTarget(rightBody);
                var colB = Column.Create(fieldB, targetB);
                valueFunction = valueFunctionHelper.ParseFunction(rightCallExpr);

                result = Conditionals.Column.Equals(col, op, valueFunction, colB);
            }

            if (invertResult) return Not.This(result);

            return result;
        }

        private static void GetCallAndNonCallExpressions(
            Expression expression,
            out MethodCallExpression callExpr,
            out Expression nonCallExpr)
        {
            callExpr = null;
            nonCallExpr = null;

            var binaryExpr = expression as BinaryExpression;
            if (binaryExpr == null)
            {
                var lambda = expression as LambdaExpression;
                binaryExpr = lambda?.Body as BinaryExpression;
            }

            if (binaryExpr == null) return;

            var left = binaryExpr.Left;
            var right = binaryExpr.Right;

            if (left.NodeType == ExpressionType.Call)
            {
                callExpr = left as MethodCallExpression;
                nonCallExpr = right;
            }
            else if (right.NodeType == ExpressionType.Call)
            {
                callExpr = right as MethodCallExpression;
                nonCallExpr = left;
            }
        }

        private IWhere ParseMethodAccessCall(Expression expression, IQueryContextExtended context)
        {
            var callExpression = expression as MethodCallExpression;
            if (callExpression == null) return null;

            IWhere result = null;
            switch(callExpression.Method.Name)
            {
                case SupportedMethods.Contains:
                    result = ParseVariableContainsAsValuesIn(callExpression, context);
                    break;
            }

            return result;
        }

        private IWhere ParseVariableContainsAsValuesIn(MethodCallExpression call, IQueryContextExtended context)
        {
            var arg = call.Arguments[0];
            if (arg.NodeType != ExpressionType.MemberAccess) return null;

            MemberExpression memberAccessExp;
            Expression valueAccessExp;

            // check if this is a list or contains with variable
            if (call.Arguments.Count != 1)
            {
                // if the underlying value is an array
                memberAccessExp = call.Arguments[1] as MemberExpression;
                valueAccessExp = call.Arguments[0];
            }
            else
            {
                // check for passed in variable
                memberAccessExp = call.Arguments[0] as MemberExpression;
                valueAccessExp = call.Object;
            }

            if (memberAccessExp == null) return null;

            // get values
            var valueMember = valueAccessExp as MemberExpression;
            object values = expressionHelper.GetValue(valueMember);

            // get alias and field name
            var fieldName = expressionHelper.GetMemberName(memberAccessExp);

            // setup arguments
            var typeArgs = new[] { memberAccessExp.Type };

            var inputGenericType = typeof(List<>);
            var inputType = inputGenericType.MakeGenericType(typeArgs);
            var input = Activator.CreateInstance(inputType, values);
            var target = context.GetQueryTarget(memberAccessExp);
            var col = Column.Create(fieldName, target);

            var parameters = new object[] { col, input };
            var enumerableGenericType = typeof(IEnumerable<>);
            var enumerableType = enumerableGenericType.MakeGenericType(typeArgs);

            // get generic type
            var toCreate = typeof(InValues<>);
            var genericToCreate = toCreate.MakeGenericType(typeArgs);
            var parameterTypes = new[] { col.GetType(), enumerableType };
            var bindingFlags = BindingFlags.Public | BindingFlags.Instance;

            var ctor = genericToCreate.GetConstructor(bindingFlags, null, parameterTypes, null);

            if (ctor == null) return null;

            // return instance
            return (IWhere)ctor.Invoke(parameters);
        }

        public static class SupportedMethods
        {
            public const string DatePart = "DatePart";
            public const string ToLower = "ToLower";
            public const string ToUpper = "ToUpper";
            public const string Contains = "Contains";
            public const string Substring = "Substring";
            public const string Trim = "Trim";
            public const string StartsWith = "StartsWith";
            public const string EndsWith = "EndsWith";
        }
    }
}
