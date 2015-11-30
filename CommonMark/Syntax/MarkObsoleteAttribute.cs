using System;

namespace CommonMark.Syntax
{
    /// <summary>
    /// Workaround for <see cref="ObsoleteAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false)]
    public class MarkObsoleteAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarkObsoleteAttribute"/> class
        /// with default properties.
        /// </summary>
        public MarkObsoleteAttribute()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkObsoleteAttribute"/> class
        /// with a specified workaround message.
        /// </summary>
        /// <param name="message">The text string that describes alternative workarounds.</param>
        public MarkObsoleteAttribute(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkObsoleteAttribute"/> class with a workaround
        /// message and a Boolean value indicating whether the obsolete element usage
        /// is considered an error.
        /// </summary>
        /// <param name="message">The text string that describes alternative workarounds.</param>
        /// <param name="error">
        /// The Boolean value that indicates whether the obsolete element usage is considered
        /// an error.
        /// </param>
        public MarkObsoleteAttribute(string message, bool error)
        {
            Message = message;
            IsError = error;
        }

        /// <summary>
        /// Gets the workaround message, including a description of the alternative program
        /// elements.
        /// </summary>
        /// <value>The workaround text string.</value>
        public string Message { get; private set; }

        /// <summary>
        /// Gets a Boolean value indicating whether the compiler will treat usage of
        /// the obsolete program element as an error.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the obsolete element usage is considered an error; otherwise, <c>false</c>.
        /// The default is <c>false</c>.
        /// </returns>
        public bool IsError { get; private set; }
    }
}
