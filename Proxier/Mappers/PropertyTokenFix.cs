using System.Reflection;

namespace Proxier.Mappers
{
    /// <summary>
    ///     PropertyInfo wrapper maintaining the token.
    /// </summary>
    public class PropertyTokenFix
    {
        /// <summary>
        ///     The propertyInfo.
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        ///     The original token.
        /// </summary>
        public int Token { get; set; }
    }
}