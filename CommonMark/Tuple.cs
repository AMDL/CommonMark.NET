namespace CommonMark
{
#if v2_0 || v3_5
    /// <summary>
    /// An alternative to <c>System.Tuple</c>, which is not present in .NET 3.5.
    /// </summary>
    public class Tuple<T1, T2>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple{T1,T2}"/> class.
        /// </summary>
        /// <param name="item1">The value of the tuple's first component.</param>
        /// <param name="item2">The value of the tuple's second component.</param>
        public Tuple(T1 item1, T2 item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }

        /// <summary>
        /// Gets the value of the current <see cref="Tuple{T1,T2}"/> object's first component.
        /// </summary>
        /// <value>The value of the current <see cref="Tuple{T1,T2}"/> object's first component.</value>
        public T1 Item1
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the value of the current <see cref="Tuple{T1,T2}"/> object's second component.
        /// </summary>
        /// <value>The value of the current <see cref="Tuple{T1,T2}"/> object's second component.</value>
        public T2 Item2
        {
            get;
            private set;
        }
    }
#endif
}
