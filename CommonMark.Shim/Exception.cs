namespace CommonMark
{
    /// <summary>
    /// <see cref="System.Exception"/> shim.
    /// </summary>
    public partial class Exception : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="System.Exception"/> class.
        /// </summary>
        public Exception()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="System.Exception"/> class with a specified
        /// error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public Exception(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="System.Exception"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public Exception(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
