using System;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace AttributeBuilder.Standard
{
    /// <summary>
    /// Provides a set of static methods for applying attributes to the Builder 
    /// classes found in the System.Reflection.Emit namespace.
    /// </summary>
    public static class BuilderExtensions
    {
        /// <summary>
        /// Applies an attribute to an assembly.
        /// </summary>
        /// <param name="assembly">An instance of AssemblyBuilder to apply the attribute to.</param>
        /// <param name="expression">An expression that represents the attribute.</param>
        public static void SetCustomAttribute(this AssemblyBuilder assembly, Expression<Func<Attribute>> expression)
        {
            var builder = assembly.ReflectionOnly
                              ? new ReflectionOnlyCustomAttributeBuilderBuilder()
                              : new CustomAttributeBuilderBuilder();
            var attribute = builder.Build(expression);
            assembly.SetCustomAttribute(attribute);
        }

        /// <summary>
        /// Applies an attribute to a constructor.
        /// </summary>
        /// <param name="constructor">An instance of ConstructorBuilder to apply the attribute to.</param>
        /// <param name="expression">An expression that represents the attribute.</param>
        public static void SetCustomAttribute(this ConstructorBuilder constructor, Expression<Func<Attribute>> expression)
        {
            var builder = constructor.Module.Assembly.ReflectionOnly
                              ? new ReflectionOnlyCustomAttributeBuilderBuilder()
                              : new CustomAttributeBuilderBuilder();
            var attribute = builder.Build(expression);
            constructor.SetCustomAttribute(attribute);
        }

        /// <summary>
        /// Applies an attribute to an enum.
        /// </summary>
        /// <param name="enum">An instance of EnumBuilder to apply the attribute to.</param>
        /// <param name="expression">An expression that represents the attribute.</param>
        public static void SetCustomAttribute(this EnumBuilder @enum, Expression<Func<Attribute>> expression)
        {
            var builder = @enum.Module.Assembly.ReflectionOnly
                              ? new ReflectionOnlyCustomAttributeBuilderBuilder()
                              : new CustomAttributeBuilderBuilder();
            var attribute = builder.Build(expression);
            @enum.SetCustomAttribute(attribute);
        }

        /// <summary>
        /// Applies an attribute to an event.
        /// </summary>
        /// <remarks>Does not work for EventBuilders created in the reflection only context.</remarks>
        /// <param name="event">An instance of AssemblyBuilder to apply the attribute to.</param>
        /// <param name="expression">An expression that represents the attribute.</param>
        public static void SetCustomAttribute(this EventBuilder @event, Expression<Func<Attribute>> expression)
        {
            var builder = new CustomAttributeBuilderBuilder();
            var attribute = builder.Build(expression);
            @event.SetCustomAttribute(attribute);
        }

        /// <summary>
        /// Applies an attribute to a field.
        /// </summary>
        /// <param name="field">An instance of FieldBuilder to apply the attribute to.</param>
        /// <param name="expression">An expression that represents the attribute.</param>
        public static void SetCustomAttribute(this FieldBuilder field, Expression<Func<Attribute>> expression)
        {
            var builder = field.Module.Assembly.ReflectionOnly
                              ? new ReflectionOnlyCustomAttributeBuilderBuilder()
                              : new CustomAttributeBuilderBuilder();
            var attribute = builder.Build(expression);
            field.SetCustomAttribute(attribute);
        }

        /// <summary>
        /// Applies an attribute to a generic type parameter.
        /// </summary>
        /// <param name="parameter">An instance of GenericTypeParameterBuilder to apply the attribute to.</param>
        /// <param name="expression">An expression that represents the attribute.</param>
        public static void SetCustomAttribute(this GenericTypeParameterBuilder parameter, Expression<Func<Attribute>> expression)
        {
            var builder = parameter.Module.Assembly.ReflectionOnly
                              ? new ReflectionOnlyCustomAttributeBuilderBuilder()
                              : new CustomAttributeBuilderBuilder();
            var attribute = builder.Build(expression);
            parameter.SetCustomAttribute(attribute);
        }

        /// <summary>
        /// Applies an attribute to a method.
        /// </summary>
        /// <param name="method">An instance of MethodBuilder to apply the attribute to.</param>
        /// <param name="expression">An expression that represents the attribute.</param>
        public static void SetCustomAttribute(this MethodBuilder method, Expression<Func<Attribute>> expression)
        {
            var builder = method.Module.Assembly.ReflectionOnly
                              ? new ReflectionOnlyCustomAttributeBuilderBuilder()
                              : new CustomAttributeBuilderBuilder();
            var attribute = builder.Build(expression);
            method.SetCustomAttribute(attribute);
        }

        /// <summary>
        /// Applies an attribute to a module.
        /// </summary>
        /// <param name="module">An instance of ModuleBuilder to apply the attribute to.</param>
        /// <param name="expression">An expression that represents the attribute.</param>
        public static void SetCustomAttribute(this ModuleBuilder module, Expression<Func<Attribute>> expression)
        {
            var builder = module.Assembly.ReflectionOnly
                              ? new ReflectionOnlyCustomAttributeBuilderBuilder()
                              : new CustomAttributeBuilderBuilder();
            var attribute = builder.Build(expression);
            module.SetCustomAttribute(attribute);
        }

        /// <summary>
        /// Applies an attribute to a method parameter.
        /// </summary>
        /// <remarks>Does not work for ParameterBuilders created in the reflection only context.</remarks>
        /// <param name="parameter">An instance of ParameterBuilder to apply the attribute to.</param>
        /// <param name="expression">An expression that represents the attribute.</param>
        public static void SetCustomAttribute(this ParameterBuilder parameter, Expression<Func<Attribute>> expression)
        {
            var builder = new CustomAttributeBuilderBuilder();
            var attribute = builder.Build(expression);
            parameter.SetCustomAttribute(attribute);
        }

        /// <summary>
        /// Applies an attribute to a property.
        /// </summary>
        /// <param name="property">An instance of PropertyBuilder to apply the attribute to.</param>
        /// <param name="expression">An expression that represents the attribute.</param>
        public static void SetCustomAttribute(this PropertyBuilder property, Expression<Func<Attribute>> expression)
        {
            var builder = property.Module.Assembly.ReflectionOnly
                              ? new ReflectionOnlyCustomAttributeBuilderBuilder()
                              : new CustomAttributeBuilderBuilder();
            var attribute = builder.Build(expression);
            property.SetCustomAttribute(attribute);
        }

        /// <summary>
        /// Applies an attribute to a type (can be class, interface, struct or delegate).
        /// </summary>
        /// <param name="type">An instance of TypeBuilder to apply the attribute to.</param>
        /// <param name="expression">An expression that represents the attribute.</param>
        public static void SetCustomAttribute(this TypeBuilder type, Expression<Func<Attribute>> expression)
        {
            var builder = type.Assembly.ReflectionOnly
                              ? new ReflectionOnlyCustomAttributeBuilderBuilder()
                              : new CustomAttributeBuilderBuilder();
            var attribute = builder.Build(expression);
            type.SetCustomAttribute(attribute);
        }
    }
}
 