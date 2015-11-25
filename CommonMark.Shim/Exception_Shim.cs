using System.Runtime.Serialization;

namespace CommonMark
{
    partial class Exception : ISerializable
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="System.Exception"/> class with serialized
        /// data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        public Exception(SerializationInfo info, StreamingContext context)
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
    }
}
