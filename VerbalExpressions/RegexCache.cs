using System;
using System.Text.RegularExpressions;

namespace CSharpVerbalExpressions
{
    public sealed class RegexCache
    {
        private bool hasValue;
        private Key key;
        private Regex regex;

        /// <summary>
        ///     Gets the already cached value for a key, or calculates the value and stores it.
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
                if (hasValue && current.Equals(key))
                    return regex;

                regex = new Regex(pattern, options);
                key = current;
                hasValue = true;
                return regex;
            }
        }

        private class Key
        {
            public Key(string pattern, RegexOptions options)
            {
                Pattern = pattern;
                Options = options;
            }

            public string Pattern { get; }
            public RegexOptions Options { get; }

            public override bool Equals(object obj)
            {
                var key = obj as Key;
                return key != null &&
                       key.Pattern == Pattern &&
                       key.Options == Options;
            }

            public override int GetHashCode()
            {
                return Pattern.GetHashCode() ^ Options.GetHashCode();
            }
        }
    }
}