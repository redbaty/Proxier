using System;
using Microsoft.CodeAnalysis;

namespace Proxier.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    ///     Represents an error during compilation
    /// </summary>
    /// <seealso cref="T:System.Exception" />
    public class CompilationError : Exception
    {
        /// <inheritdoc />
        public CompilationError(Diagnostic diagnostic, string message)
        {
            Diagnostic = diagnostic;
            Message = message;
        }

        /// <summary>
        ///     Gets the diagnostic information.
        /// </summary>
        /// <value>
        ///     The diagnostic.
        /// </value>
        public Diagnostic Diagnostic { get; }

        /// <summary>
        ///     Gets a message that describes the current exception.
        /// </summary>
        public override string Message { get; }
    }
}