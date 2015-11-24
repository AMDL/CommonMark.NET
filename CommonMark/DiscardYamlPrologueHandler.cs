namespace CommonMark
{
    /// <summary>
    /// <see cref="PrologueLineHandler"/> to discard YAML metadata blocks.
    /// </summary>
    public class DiscardYamlPrologueHandler
    {
        private bool isInYaml = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscardYamlPrologueHandler"/> class.
        /// </summary>
        /// <param name="line">Single input line.</param>
        /// <returns><c>true</c> if the prologue is incomplete, or <c>false</c> otherwise.</returns>
        public bool HandlePrologue(ref string line)
        {
            if (line == null)
                return false;

            if (IsNullOrWhiteSpace(line))
                return true;

            if (IsYamlDelimiter(line))
            {
                if (!isInYaml)
                {
                    isInYaml = true;
                }
                else
                {
                    isInYaml = false; //Reusable handler
                    line = null; //Remove from output
                }
            }

            return isInYaml;
        }

        private static bool IsYamlDelimiter(string line)
        {
            var trimLine = line.TrimEnd();
            return trimLine.Equals("---") || trimLine.Equals("...");
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        static bool IsNullOrWhiteSpace(string str)
        {
#if v2_0 || v3_5
            return string.IsNullOrEmpty(str?.Trim());
#else
            return string.IsNullOrWhiteSpace(str);
#endif
        }
    }
}
