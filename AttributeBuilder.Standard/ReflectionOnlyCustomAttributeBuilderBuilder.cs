using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AttributeBuilder.Standard
{
    /// <summary>
    ///     Able to create a CustomAttributeBuilder instance from a lambda expression
    ///     in reflection only scenarios where the type of the attribute is loaded
    ///     into the reflection only context.
    /// </summary>
    internal class ReflectionOnlyCustomAttributeBuilderBuilder : CustomAttributeBuilderBuilder
    {
        protected override void ProcessNew(NewExpression expression)
        {
            base.ProcessNew(expression);

            if (expression.Constructor.DeclaringType != null)
            {
                var assembly =
                    Assembly.ReflectionOnlyLoad(expression.Constructor.DeclaringType.Assembly.FullName);

                Constructor = assembly
                    .GetType(expression.Constructor.DeclaringType.FullName)
                    .GetConstructor(ConstructorArgs.Select(arg => arg.GetType()).ToArray());
            }
        }
    }
}