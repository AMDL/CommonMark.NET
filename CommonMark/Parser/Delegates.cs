using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser
{
    /// <summary>
    /// Stage 1 block advancer delegate.
    /// </summary>
    /// <param name="container">Container element.</param>
    /// <param name="line">Line string.</param>
    /// <param name="first_nonspace">The index of the first non-space character.</param>
    /// <param name="offset">Offset.</param>
    /// <param name="column">Column index.</param>
    /// <param name="indent">Line indent.</param>
    /// <param name="blank"><c>true</c> if the line is blank.</param>
    /// <returns><c>true</c> if successful.</returns>
    public delegate bool BlockAdvancerDelegate(Block container, string line, int first_nonspace, ref int offset, ref int column, int indent, bool blank);

    /// <summary>
    /// Stage 1 block initializer delegate.
    /// </summary>
    /// <param name="container">Container element.</param>
    /// <param name="lineInfo">Line info.</param>
    /// <param name="line">Line string.</param>
    /// <param name="first_nonspace">The index of the first non-space character.</param>
    /// <param name="indent">Line indent.</param>
    /// <param name="indented"><c>true</c> if the line is indented.</param>
    /// <param name="all_matched"><c>true</c> if all matched.</param>
    /// <param name="offset">Offset.</param>
    /// <param name="column">Column index.</param>
    /// <param name="settings">Common settings.</param>
    /// <returns><c>true</c> if successful.</returns>
    public delegate bool BlockInitializerDelegate(ref Block container, LineInfo lineInfo, string line, int first_nonspace, int indent, bool indented, bool all_matched, ref int offset, ref int column, CommonMarkSettings settings);

    /// <summary>
    /// Stage 1 block finalizer delegate.
    /// </summary>
    /// <param name="container">Container element.</param>
    /// <param name="lineInfo">Line info.</param>
    /// <param name="line">Line string.</param>
    /// <param name="first_nonspace">The index of the first non-space character.</param>
    /// <param name="offset">Offset.</param>
    /// <param name="indented"><c>true</c> if the line is indented.</param>
    /// <param name="blank"><c>true</c> if the line is blank.</param>
    /// <param name="settings">Common settings.</param>
    /// <returns><c>true</c> if successful.</returns>
    public delegate bool BlockFinalizerDelegate(ref Block container, LineInfo lineInfo, string line, int first_nonspace, int offset, bool indented, bool blank, CommonMarkSettings settings);

    /// <summary>
    /// Stage 2 block processor delegate.
    /// </summary>
    /// <param name="container">Container element.</param>
    /// <param name="subject">Subject.</param>
    /// <param name="referenceMap">The reference mapping used when parsing links.</param>
    /// <param name="inlineStack">Inline stack.</param>
    /// <param name="settings">Common settings.</param>
    /// <returns><c>true</c> if successful.</returns>
    public delegate bool BlockProcessorDelegate(Block container, Subject subject, Dictionary<string, Reference> referenceMap, ref Stack<Inline> inlineStack, CommonMarkSettings settings);

    /// <summary>
    /// Inline parser delegate.
    /// </summary>
    /// <param name="parent">Parent block.</param>
    /// <param name="subject">Subject.</param>
    /// <returns>Inline element or <c>null</c>.</returns>
    public delegate Inline InlineParserDelegate(Block parent, Subject subject);

    /// <summary>
    /// Inline stack delimiter matcher delegate.
    /// </summary>
    /// <param name="subj">The source subject.</param>
    /// <param name="startIndex">The index of the first character.</param>
    /// <param name="length">The length of the substring.</param>
    /// <param name="beforeIsPunctuation"><c>true</c> if the substring is preceded by a punctuation character.</param>
    /// <param name="afterIsPunctuation"><c>true</c> if the substring is followed by a punctuation character.</param>
    /// <param name="canOpen"><c>true</c> if the delimiter can serve as an opener.</param>
    /// <param name="canClose"><c>true</c> if the delimiter can serve as a closer.</param>
    public delegate void InlineDelimiterMatcherDelegate(Subject subj, int startIndex, int length, bool beforeIsPunctuation, bool afterIsPunctuation, ref bool canOpen, ref bool canClose);

    /// <summary>
    /// String normalizer delegate.
    /// </summary>
    /// <param name="s">String.</param>
    /// <returns>Normalized string.</returns>
    public delegate string StringNormalizerDelegate(string s);
}
