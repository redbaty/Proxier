using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Proxier.Representations
{
    /// <summary>
    ///     This is a property's representation
    /// </summary>
    public class PropertyRepresentation
    {
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

        private IEnumerable<Expression<Func<Attribute>>> Attributes { get; }

        private IEnumerable<Attribute> CompiledAttributes { get; }

        private bool IsReadOnly { get; }

        private string Name { get; }

        private Type Type { get; }

        private bool IsInterface { get; }

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
                    stringBuilder.AppendLine($"\t[{compiledAttribute}]");

            stringBuilder.AppendLine(
                $"\t{(IsInterface ? string.Empty : "public ")}{GetResolvedTypeName(Type)} {Name} {{ get;{(IsReadOnly ? string.Empty : " set;")} }}");

            return stringBuilder.ToString();
        }

        private string GetResolvedTypeName(Type type)
        {
            var isNullable = false;

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
    }
}