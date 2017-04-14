/*
 * SonarQube, open source software quality management tool.
 * Copyright (C) 2008-2013 SonarSource
 * mailto:contact AT sonarsource DOT com
 *
 * SonarQube is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * SonarQube is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Text.RegularExpressions;

namespace CSharpVerbalExpressions
{
    public sealed class RegexCache
    {
        private static readonly object lock_object = new object();

        private bool hasValue;
        private Key key;
        private Regex regex;
        
        private class Key : IEquatable<Key>
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

            public bool Equals(Key other)
            {
                return other.Pattern == this.Pattern && other.Options == this.Options;
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
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }

            lock (lock_object)
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
