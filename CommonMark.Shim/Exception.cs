using System.Runtime.Serialization;

namespace CommonMark
{
    /// <summary>
    /// <see cref="System.Exception"/> shim.
    /// </summary>
    public class Exception : System.Exception
#if !DOTNET
        , ISerializable
#endif
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

#if !DOTNET
        /// <summary>
        /// Initializes a new instance of the <see cref="System.Exception"/> class with serialized
        /// data.
        /// </summary>
        /// <param name="serializationInfo"></param>
        /// <param name="streamingContext"></param>
        public Exception(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="SerializationInfo"/>
        /// with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }
#endif
    }
}
