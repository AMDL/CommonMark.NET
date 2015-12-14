using System;

namespace CommonMark.Parser
{
    /// <summary>
    /// Line information.
    /// </summary>
    public sealed class LineInfo
    {
        internal LineInfo(bool trackPositions)
        {
            this.IsTrackingPositions = trackPositions;
        }

        /// <summary>
        /// Determines whether position tracking is enabled.
        /// </summary>
        public readonly bool IsTrackingPositions;

        /// <summary>
        /// Line string.
        /// </summary>
        public string Line;

        /// <summary>
        /// Gets or sets the offset in the source data at which the current line starts.
        /// </summary>
        public int LineOffset;

        /// <summary>
        /// Line number.
        /// </summary>
        public int LineNumber;

        internal PositionOffset[] Offsets = new PositionOffset[20];

        internal int OffsetCount;

        /// <summary>
        /// Adds a position offset.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="offset">Offset.</param>
        public void AddOffset(int position, int offset)
        {
            if (offset == 0)
                return;

            if (this.OffsetCount == this.Offsets.Length)
                Array.Resize(ref this.Offsets, this.OffsetCount + 20);

            this.Offsets[this.OffsetCount++] = new PositionOffset(position, offset);
        }

        /// <summary>
        /// Converts the instance to string.
        /// </summary>
        /// <returns>String.</returns>
        public override string ToString()
        {
            string ln;
            if (this.Line == null)
                ln = string.Empty;
            else if (this.Line.Length < 50)
                ln = this.Line;
            else
                ln = this.Line.Substring(0, 49) + "…";

            return this.LineNumber.ToString(System.Globalization.CultureInfo.InvariantCulture)
                + ": " + ln;
        }

        /// <summary>
        /// Returns the origin of the specified position.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="isStartPosition"><c>true</c> if <paramref name="position"/> is a start position.</param>
        /// <returns>Origin.</returns>
        public int CalculateOrigin(int position, bool isStartPosition)
        {
            return PositionTracker.CalculateOrigin(this.Offsets, this.OffsetCount, this.LineOffset + position, true, isStartPosition);
        }
    }
}
