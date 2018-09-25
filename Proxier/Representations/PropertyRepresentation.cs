using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Proxier.Representations
{
    /// <summary>
    ///     This is a property's representation
    /// </summary>
    public class PropertyRepresentation
    {
        private IEnumerable<Expression<Func<Attribute>>> Attributes { get; }

        private IEnumerable<Attribute> CompiledAttributes { get; }

        private bool IsInterface { get; }

        private bool IsReadOnly { get; }

        private string Name { get; }

        private Type Type { get; }

        /// <inheritdoc />
        public PropertyRepresentation(string name, Type type, bool isReadOnly,
            IEnumerable<Expression<Func<Attribute>>> attributes,
            IEnumerable<Attribute> compiledAttributes, bool isInterface)
        {
            Name = name;
            Type = type;
            Attributes = attributes;
            CompiledAttributes = compiledAttributes;
            IsInterface = isInterface;
            IsReadOnly = isReadOnly;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            if (Attributes != null && Attributes.Any())
                foreach (var expression in Attributes)
                {
                    if (!(expression.Body is NewExpression newExpression)) continue;

                    foreach (var newExpressionArgument in newExpression.Arguments)
                        if (newExpressionArgument is ConstantExpression constantExpression)
                            stringBuilder.AppendLine(
                                $"\t[{expression.Compile().Invoke()}(\"{constantExpression.Value}\")]");
                }

            if (CompiledAttributes != null && CompiledAttributes.Any())
                foreach (var compiledAttribute in CompiledAttributes)
                {
                    var aggregate = compiledAttribute.GetType()
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(i => i.PropertyType.Namespace != null && i.PropertyType.Namespace.StartsWith("System") &&
                                    i.CanWrite)
                        .Select(i => new {i.Name, Value = i.GetValue(compiledAttribute)})
                        .Where(i => i.Value != null)
                        .Select(i => i.Value is string ? $"{i.Name}=\"{i.Value}\"" : $"{i.Name}={i.Value}")
                        .Aggregate((x, y) => $"{x}, {y}");
                    stringBuilder.AppendLine($"\t[{compiledAttribute}({aggregate})]");
                }

            stringBuilder.AppendLine(
                $"\t{(IsInterface ? string.Empty : "public ")}{GetResolvedTypeName(Type)} {Name} {{ get;{(IsReadOnly ? string.Empty : " set;")} }}");

            return stringBuilder.ToString();
        }

        private string GetResolvedTypeName(Type type)
        {
            var isNullable = false;

            if (type.IsGenericType && Nullable.GetUnderlyingType(type) == null) return GetRealName(type);

            while (true)
            {
                if (Nullable.GetUnderlyingType(type ?? throw new ArgumentNullException(nameof(type))) != null)
                {
                    isNullable = true;
                    type = Nullable.GetUnderlyingType(type);
                    continue;
                }

                return type.Name + (isNullable ? "?" : "");
            }
        }

        private static string GetRealName(Type type)
        {
            if (type.IsGenericType && Nullable.GetUnderlyingType(type) == null)
            {
                var resolvedTypeName =
                    $"{type.Name.Substring(0, type.Name.IndexOf("`", StringComparison.Ordinal))}<{type.GetGenericArguments().Select(GetRealName).Aggregate((x, y) => $"{x}, {y}")}>";
                return resolvedTypeName;
            }

            return type.Name;
        }
    }
}