using CommonMark.Syntax;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Parser
{
    /// <summary>
    /// Inline parser subject.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{DebugToString()}")]
    public sealed class Subject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Subject"/> class.
        /// </summary>
        /// <param name="referenceMap">Reference map.</param>
        public Subject(Dictionary<string, Reference> referenceMap)
        {
            this.ReferenceMap = referenceMap;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Subject"/> class.
        /// </summary>
        /// <param name="buffer">String buffer.</param>
        /// <param name="referenceMap">Reference map.</param>
        public Subject(string buffer, Dictionary<string, Reference> referenceMap)
        {
            this.Buffer = buffer;
            this.Length = buffer.Length;
            this.ReferenceMap = referenceMap;
        }

#if DEBUG
        public int DebugStartIndex;
#endif

        /// <summary>
        /// Gets or sets the whole buffer this instance is created over.
        /// </summary>
        public string Buffer;

        /// <summary>
        /// Gets or sets the current position in the buffer.
        /// </summary>
        public int Position;

        /// <summary>
        /// Gets or sets the length of the usable buffer. This can be less than the actual length of the
        /// buffer if some characters at the end of the buffer have to be ignored.
        /// </summary>
        public int Length;

        /// <summary>
        /// The last top-level inline parsed from this subject.
        /// </summary>
        public Inline LastInline;

        /// <summary>
        /// The last entry of the current stack of possible emphasis openers. Can be <c>null</c>.
        /// </summary>
        public InlineStack LastPendingInline;

        /// <summary>
        /// The first entry of the current stack of possible emphasis openers. Can be <c>null</c>.
        /// </summary>
        public InlineStack FirstPendingInline;

        /// <summary>
        /// A reusable StringBuilder that should be used instead of creating new instances to conserve memory.
        /// </summary>
        public StringBuilder ReusableStringBuilder = new StringBuilder();

        /// <summary>
        /// A mapping from reference labels to reference link definitions.
        /// </summary>
        public readonly Dictionary<string, Reference> ReferenceMap;

#if !NETCore
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by [DebuggerDisplay]")]
#endif
        // ReSharper disable once UnusedMethodReturnValue.Local
        private string DebugToString()
        {
            var res = this.Buffer.Insert(this.Length, "|");
            res = res.Insert(this.Position, "⁞");

#if DEBUG
            res = res.Insert(this.DebugStartIndex, "|");
#endif

            return res;
        }
    }
}
