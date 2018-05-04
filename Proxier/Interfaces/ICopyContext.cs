using System.Reflection;

namespace Proxier.Interfaces
{
    /// <summary>
    ///     Copy context
    /// </summary>
    public interface ICopyContext
    {
        /// <summary>
        ///     Gets the property.
        /// </summary>
        /// <value>
        ///     The property.
        /// </value>
        PropertyInfo Property { get; }

        /// <summary>
        ///     Gets the source.
        /// </summary>
        /// <value>
        ///     The source.
        /// </value>
        object Source { get; }

        /// <summary>
        ///     Gets the target.
        /// </summary>
        /// <value>
        ///     The target.
        /// </value>
        object Target { get; }
    }
}