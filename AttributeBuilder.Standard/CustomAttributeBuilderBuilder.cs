using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace AttributeBuilder.Standard
{
    /// <summary>
    /// Able to create a CustomAttributeBuilder instance from a lambda expression.
    /// </summary>
    internal class CustomAttributeBuilderBuilder
    {
        private readonly List<object> _fieldValues = new List<object>();
        private readonly List<FieldInfo> _namedFields = new List<FieldInfo>();
        private readonly List<PropertyInfo> _namedProperties = new List<PropertyInfo>();
        private readonly List<object> _propertyValues = new List<object>();
        protected readonly List<object> ConstructorArgs = new List<object>();
        protected ConstructorInfo Constructor;

        /// <summary>
        /// Creates a CustomAttributeBuilder object based on a lambda expression,
        /// matching the type of attribute, the constructor and its arguments and
        /// the named parameters.
        /// </summary>
        /// <param name="expression">
        /// A lamda expression of type Expression.New or
        /// Expression.MemberInit.
        /// </param>
        /// <returns>
        /// A CustomAttributeBuilder with the same properties as the attribute
        /// constructed in the lambda expression.
        /// </returns>
        public CustomAttributeBuilder Build(Expression<Func<Attribute>> expression)
        {
            Process(expression);

            return new CustomAttributeBuilder(Constructor, ConstructorArgs.ToArray(),
                _namedProperties.ToArray(), _propertyValues.ToArray(),
                _namedFields.ToArray(), _fieldValues.ToArray());
        }

        private void Process(LambdaExpression expression)
        {
            switch (expression.Body.NodeType)
            {
                case ExpressionType.MemberInit:
                    ProcessMemberInit((MemberInitExpression)expression.Body);
                    break;
                case ExpressionType.New:
                    ProcessNew((NewExpression)expression.Body);
                    break;
                default:
                    throw new ArgumentException(Resources.UnSupportedExpression, nameof(expression));
            }
        }

        private void ProcessMemberInit(MemberInitExpression expression)
        {
            ProcessNew(expression.NewExpression);

            var bindings = from b in expression.Bindings
                where b.BindingType == MemberBindingType.Assignment
                select b;

            foreach (var binding in bindings)
            {
                var memberAssignment = (MemberAssignment)binding;
                var lambda = Expression.Lambda(memberAssignment.Expression);
                var value = lambda.Compile().DynamicInvoke();

                if (memberAssignment.Member is PropertyInfo)
                {
                    _namedProperties.Add((PropertyInfo)memberAssignment.Member);
                    _propertyValues.Add(value);
                }
                else if (memberAssignment.Member is FieldInfo)
                {
                    _namedFields.Add((FieldInfo)memberAssignment.Member);
                    _fieldValues.Add(value);
                }
            }
        }

        protected virtual void ProcessNew(NewExpression expression)
        {
            Constructor = expression.Constructor;

            foreach (var arg in expression.Arguments)
            {
                var lambda = Expression.Lambda(arg);
                var value = lambda.Compile().DynamicInvoke();
                ConstructorArgs.Add(value);
            }
        }
    }
}
