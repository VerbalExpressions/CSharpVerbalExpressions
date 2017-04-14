using System;
using System.Text.RegularExpressions;

namespace CSharpVerbalExpressions
{
    public sealed class RegexCache
    {
        private bool hasValue;
        private Key key;
        private Regex regex;
        
        private class Key
        {
            public Key(string pattern, RegexOptions options)
            {
                this.Pattern = pattern;
                this.Options = options;
            }

            public string Pattern { get; private set; }
            public RegexOptions Options { get; private set; }
            
            public override bool Equals(object obj)
            {
                var key = obj as Key;
                return key != null &&
                       key.Pattern == this.Pattern &&
                       key.Options == this.Options;
            }

            public override int GetHashCode()
            {
                return this.Pattern.GetHashCode() ^ this.Options.GetHashCode();
            }
        }

        /// <summary>
        /// Gets the already cached value for a key, or calculates the value and stores it.
        /// </summary>
        /// <param name="pattern">The pattern used to create the regular expression.</param>
        /// <param name="options">The options for regex.</param>
        /// <returns>The calculated or cached value.</returns>
        public Regex Get(string pattern, RegexOptions options)
        {
            if (pattern == null) throw new ArgumentNullException("pattern");

            lock (this)
            {
                var current = new Key(pattern, options);
                if (this.hasValue && current.Equals(this.key))
                {
                    return this.regex;
                }

                this.regex = new Regex(pattern, options);
                this.key = current;
                this.hasValue = true;
                return this.regex;
            }
        }
    }
}
