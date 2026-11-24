/*!
 * CSharpVerbalExpressions
 * https://github.com/VerbalExpressions/CSharpVerbalExpressions
 *
 * @psoholt
 *
 * Date: 2013-07-26
 *
 * Additions and Refactoring
 * @alexpeta
 *
 * Date: 2013-08-06
 */

using System;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace CSharpVerbalExpressions
{
    /// <summary>
    /// A fluent builder for regular expressions. Chain the methods to describe
    /// the pattern in plain words, then test input with <see cref="Test"/> or
    /// get the underlying <see cref="Regex"/> with <see cref="ToRegex"/>.
    /// </summary>
    /// <remarks>
    /// Unless stated otherwise, string arguments are escaped with <see cref="Sanitize"/>
    /// so they are matched literally. Pass <c>sanitize: false</c> to insert raw regex syntax.
    /// The expression uses <see cref="RegexOptions.Multiline"/> by default.
    /// </remarks>
    public class VerbalExpressions
    {
        #region Statics

        /// <summary>
        /// Returns a new, empty expression with the <see cref="RegexOptions.Multiline"/> option enabled.
        /// </summary>
        public static VerbalExpressions DefaultExpression
        {
            get { return new VerbalExpressions(); }
        }

        #endregion Statics

        #region Private Members

        private readonly RegexCache regexCache = new RegexCache();
        private readonly RegexCache exactRegexCache = new RegexCache();
        private StringBuilder _prefixes = new StringBuilder();
        private StringBuilder _source = new StringBuilder();
        private StringBuilder _suffixes = new StringBuilder();

        private RegexOptions _modifiers = RegexOptions.Multiline;

        #endregion Private Members

        #region Private Properties

        private string RegexString
        {
            get { return new StringBuilder().Append(_prefixes).Append(_source).Append(_suffixes).ToString();}
        }

        private Regex PatternRegex
        {
            get { return regexCache.Get(this.RegexString, _modifiers); }
        }

        #endregion Private Properties

        #region Public Methods

        #region Helpers

        /// <summary>
        /// Escapes regex metacharacters in <paramref name="value"/> so it is matched literally.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null or empty.</exception>
        public string Sanitize(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            return Regex.Escape(value);
        }

        /// <summary>
        /// Returns true if the expression matches anywhere in <paramref name="toTest"/>.
        /// Same as <see cref="IsMatch"/>.
        /// </summary>
        public bool Test(string toTest)
        {
            return IsMatch(toTest);
        }

        /// <summary>
        /// Returns true if the expression matches anywhere in <paramref name="toTest"/>.
        /// </summary>
        public bool IsMatch(string toTest)
        {
            return PatternRegex.IsMatch(toTest);
        }

        /// <summary>
        /// Returns true only if the expression matches the whole of <paramref name="toTest"/>,
        /// from the first to the last character. Returns false for null.
        /// </summary>
        public bool TestExact(string toTest)
        {
            if (toTest == null)
            {
                return false;
            }

            // In IgnorePatternWhitespace mode a trailing # comment would swallow the
            // closing wrapper, so end the comment with a newline (ignored in that mode).
            var pattern = this.RegexString;
            if ((_modifiers & RegexOptions.IgnorePatternWhitespace) != 0)
            {
                pattern += "\n";
            }

            var exactPattern = @"\A(?:" + pattern + @")\z";
            return exactRegexCache.Get(exactPattern, _modifiers).IsMatch(toTest);
        }

        /// <summary>
        /// Returns the compiled <see cref="Regex"/> for the expression built so far.
        /// </summary>
        public Regex ToRegex()
        {
            return PatternRegex;
        }

        /// <summary>
        /// Returns the regular expression pattern built so far.
        /// </summary>
        public override string ToString()
        {
            return PatternRegex.ToString();
        }

        /// <summary>
        /// Returns the value of the named capture group in the first match in
        /// <paramref name="toTest"/>, or null if the expression does not match.
        /// </summary>
        /// <seealso cref="BeginCapture(string)"/>
        public string Capture(string toTest, string groupName)
        {
            if (!Test(toTest))
                return null;

            var match = PatternRegex.Match(toTest);
            return match.Groups[groupName].Value;
        }

        /// <summary>
        /// Replaces every match in <paramref name="input"/> with <paramref name="replacement"/>
        /// and returns the result. The replacement may use substitutions such as <c>$1</c>.
        /// </summary>
        public string Replace(string input, string replacement)
        {
            return PatternRegex.Replace(input, replacement);
        }

        #endregion Helpers

        #region Expression Modifiers

        /// <summary>
        /// Appends a predefined pattern from <see cref="CommonRegex"/>.
        /// </summary>
        public VerbalExpressions Add(CommonRegex commonRegex)
        {
            return Add(commonRegex.Name, false);
        }

        /// <summary>
        /// Appends <paramref name="value"/> to the expression without wrapping it in a group.
        /// </summary>
        /// <param name="value">The text to append.</param>
        /// <param name="sanitize">If true, <paramref name="value"/> is escaped and matched literally.</param>
        public VerbalExpressions Add(string value, bool sanitize = true)
        {
            if (value == null)
                throw new ArgumentNullException("value must be provided");

            value = sanitize ? Sanitize(value) : value;
            _source.Append(value);
            return this;
        }

        /// <summary>
        /// Appends the full pattern of another expression, wrapped in a non-capturing group.
        /// Use it to build reusable parts. The other expression is not changed, and its
        /// options (such as case-insensitivity) are not copied.
        /// </summary>
        public VerbalExpressions Add(VerbalExpressions expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            return Add("(?:" + expression.RegexString + ")", false);
        }

        /// <summary>
        /// Anchors the expression to the start of a line (<c>^</c>).
        /// </summary>
        public VerbalExpressions StartOfLine(bool enable = true)
        {
            _prefixes.Append(enable ? "^" : String.Empty);
            return this;
        }

        /// <summary>
        /// Anchors the expression to the end of a line (<c>$</c>).
        /// </summary>
        public VerbalExpressions EndOfLine(bool enable = true)
        {
            _suffixes.Append(enable ? "$" : String.Empty);
            return this;
        }

        /// <summary>
        /// Matches <paramref name="value"/> exactly, as a capture group.
        /// </summary>
        public VerbalExpressions Then(string value, bool sanitize = true)
        {
            var sanitizedValue = sanitize ? Sanitize(value) : value;
            value = string.Format("({0})", sanitizedValue);
            return Add(value, false);
        }

        /// <summary>
        /// Matches a predefined pattern from <see cref="CommonRegex"/>, as a capture group.
        /// </summary>
        public VerbalExpressions Then(CommonRegex commonRegex)
        {
            return Then(commonRegex.Name, false);
        }

        /// <summary>
        /// Same as <see cref="Then(string, bool)"/>. Reads better at the start of an expression.
        /// </summary>
        public VerbalExpressions Find(string value)
        {
            return Then(value);
        }

        /// <summary>
        /// Optionally matches <paramref name="value"/> (zero or one time), as a capture group.
        /// </summary>
        public VerbalExpressions Maybe(string value, bool sanitize = true)
        {
            value = sanitize ? Sanitize(value) : value;
            value = string.Format("({0})?", value);
            return Add(value, false);
        }

        /// <summary>
        /// Optionally matches a predefined pattern from <see cref="CommonRegex"/>.
        /// </summary>
        public VerbalExpressions Maybe(CommonRegex commonRegex)
        {
            return Maybe(commonRegex.Name, sanitize: false);
        }

        /// <summary>
        /// Matches any characters, including none (<c>(.*)</c>).
        /// </summary>
        public VerbalExpressions Anything()
        {
            return Add("(.*)", false);
        }

        /// <summary>
        /// Matches any characters except those in <paramref name="value"/>, including none.
        /// </summary>
        public VerbalExpressions AnythingBut(string value, bool sanitize = true)
        {
            value = sanitize ? SanitizeCharacterSet(value) : value;
            value = string.Format("([^{0}]*)", value);
            return Add(value, false);
        }

        /// <summary>
        /// Matches at least one character of any kind (<c>(.+)</c>).
        /// </summary>
        public VerbalExpressions Something()
        {
            return Add("(.+)", false);
        }

        /// <summary>
        /// Matches at least one character, none of which are in <paramref name="value"/>.
        /// </summary>
        public VerbalExpressions SomethingBut(string value, bool sanitize = true)
        {
            value = sanitize ? SanitizeCharacterSet(value) : value;
            return Add("([^" + value + "]+)", false);
        }

        /// <summary>
        /// Replaces the text of the current pattern inside the expression's source with
        /// <paramref name="value"/>. Kept for backward compatibility; to replace matches in a
        /// string, use <see cref="Replace(string, string)"/>.
        /// </summary>
        public VerbalExpressions Replace(string value)
        {
            string whereToReplace = PatternRegex.ToString();

            if (whereToReplace.Length != 0)
            {
                _source.Replace(whereToReplace, value);
            }

            return this;
        }

        /// <summary>
        /// Matches a line break: <c>\n</c> or <c>\r\n</c>.
        /// </summary>
        public VerbalExpressions LineBreak()
        {
            return Add(@"(\n|(\r\n))", false);
        }

        /// <summary>
        /// Same as <see cref="LineBreak"/>.
        /// </summary>
        public VerbalExpressions Br()
        {
            return LineBreak();
        }

        /// <summary>
        /// Matches a tab character (<c>\t</c>).
        /// </summary>
        public VerbalExpressions Tab()
        {
            return Add(@"\t", false);
        }

        /// <summary>
        /// Matches one or more word characters (<c>\w+</c>).
        /// </summary>
        public VerbalExpressions Word()
        {
            return Add(@"\w+", false);
        }

        /// <summary>
        /// Matches a single character that is one of the characters in <paramref name="value"/>.
        /// </summary>
        public VerbalExpressions AnyOf(string value, bool sanitize = true)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            value = sanitize ? SanitizeCharacterSet(value) : value;
            value = string.Format("[{0}]", value);
            return Add(value, false);
        }

        /// <summary>
        /// Same as <see cref="AnyOf"/>.
        /// </summary>
        public VerbalExpressions Any(string value)
        {
            return AnyOf(value);
        }

        /// <summary>
        /// Matches a single character in one or more ranges, given as pairs:
        /// <c>Range('a', 'z', '0', '9')</c> gives <c>[a-z0-9]</c>.
        /// </summary>
        public VerbalExpressions Range(params object[] arguments)
        {
            if (object.ReferenceEquals(arguments, null))
            {
                throw new ArgumentNullException("arguments");
            }

            if (arguments.Length == 1)
            {
                throw new ArgumentOutOfRangeException("arguments");
            }

            string[] sanitizedStrings = arguments.Select(argument =>
            {
                if (object.ReferenceEquals(argument, null))
                {
                    return string.Empty;
                }

                string casted = argument.ToString();
                if (string.IsNullOrEmpty(casted))
                {
                    return string.Empty;
                }
                else
                {
                    return Sanitize(casted);
                }
            })
                .Where(sanitizedString =>
                    !string.IsNullOrEmpty(sanitizedString))
                .ToArray();

            if (!sanitizedStrings.Any())
            {
                return this;
            }

            bool hasOddNumberOfParams = (sanitizedStrings.Length % 2) > 0;

            StringBuilder sb = new StringBuilder("[");
            for (int _from = 0; _from < sanitizedStrings.Length; _from += 2)
            {
                int _to = _from + 1;
                if (sanitizedStrings.Length <= _to)
                {
                    break;
                }

                string left = sanitizedStrings[_from];
                string right = sanitizedStrings[_to];

                // Sort within pair: use numeric comparison if both are numbers,
                // otherwise fall back to ordinal string comparison
                int leftNum, rightNum;
                bool shouldSwap;
                if (int.TryParse(left, out leftNum) && int.TryParse(right, out rightNum))
                {
                    shouldSwap = leftNum > rightNum;
                }
                else
                {
                    shouldSwap = string.Compare(left, right, StringComparison.Ordinal) > 0;
                }

                if (shouldSwap)
                {
                    sb.AppendFormat("{0}-{1}", right, left);
                }
                else
                {
                    sb.AppendFormat("{0}-{1}", left, right);
                }
            }
            sb.Append("]");

            if (hasOddNumberOfParams)
            {
                sb.AppendFormat("|{0}", sanitizedStrings.Last());
            }

            return Add(sb.ToString(), false);
        }

        /// <summary>
        /// Matches <paramref name="value"/> one or more times, as a capture group.
        /// </summary>
        public VerbalExpressions Multiple(string value, bool sanitize = true)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            value = sanitize ? this.Sanitize(value) : value;
            value = string.Format(@"({0})+", value);

            return Add(value, false);
        }

        /// <summary>
        /// Matches <paramref name="value"/> at least <paramref name="min"/> times, as a capture group.
        /// </summary>
        public VerbalExpressions Multiple(string value, int min)
        {
            ValidateCount(min, "min");
            return Add("(" + Sanitize(value) + ")", false).AtLeast(min);
        }

        /// <summary>
        /// Matches <paramref name="value"/> between <paramref name="min"/> and
        /// <paramref name="max"/> times, as a capture group.
        /// </summary>
        public VerbalExpressions Multiple(string value, int min, int max)
        {
            ValidateCount(min, "min");
            ValidateCount(max, "max");
            if (max < min)
            {
                throw new ArgumentOutOfRangeException("max", "max must be greater than or equal to min");
            }

            return Add("(" + Sanitize(value) + ")", false).RepeatPrevious(min, max);
        }

        /// <summary>
        /// Adds an alternative: the expression matches either what came before or
        /// a predefined pattern from <see cref="CommonRegex"/>.
        /// </summary>
        public VerbalExpressions Or(CommonRegex commonRegex)
        {
            return Or(commonRegex.Name, false);
        }

        /// <summary>
        /// Adds an alternative: the expression matches either what came before or <paramref name="value"/>.
        /// </summary>
        public VerbalExpressions Or(string value, bool sanitize = true)
        {
            _prefixes.Append("(");
            _suffixes.Insert(0, ")");
            _source.Append(")|(");
            return Add(value, sanitize);
        }

        /// <summary>
        /// Matches exactly one of <paramref name="values"/>, in a non-capturing group:
        /// <c>OneOf("cat", "dog")</c> gives <c>(?:cat|dog)</c>. Each value is escaped.
        /// </summary>
        /// <exception cref="ArgumentException">No values were given.</exception>
        public VerbalExpressions OneOf(params string[] values)
        {
            if (values == null || values.Length == 0)
            {
                throw new ArgumentException("At least one value must be provided", "values");
            }

            return Add("(?:" + string.Join("|", values.Select(Sanitize).ToArray()) + ")", false);
        }

        /// <summary>
        /// Starts a numbered capture group. Close it with <see cref="EndCapture"/>.
        /// </summary>
        public VerbalExpressions BeginCapture()
        {
            return Add("(", false);
        }

        /// <summary>
        /// Starts a named capture group. Close it with <see cref="EndCapture"/> and read
        /// the value with <see cref="Capture(string, string)"/>.
        /// </summary>
        public VerbalExpressions BeginCapture(string groupName)
        {
            return Add("(?<", false).Add(groupName, true).Add(">", false);
        }

        /// <summary>
        /// Closes a group started with <see cref="BeginCapture()"/> or <see cref="BeginCapture(string)"/>.
        /// </summary>
        public VerbalExpressions EndCapture()
        {
            return Add(")", false);
        }

        /// <summary>
        /// Starts a non-capturing group (<c>(?:</c>), useful for applying a quantifier
        /// to several parts at once. Close it with <see cref="EndGroup"/>.
        /// </summary>
        public VerbalExpressions BeginGroup()
        {
            return Add("(?:", false);
        }

        /// <summary>
        /// Closes a group started with <see cref="BeginGroup"/>.
        /// </summary>
        public VerbalExpressions EndGroup()
        {
            return Add(")", false);
        }

        /// <summary>
        /// Repeats the previous item exactly <paramref name="n"/> times (<c>{n}</c>).
        /// </summary>
        public VerbalExpressions RepeatPrevious(int n)
        {
            return Add("{" + n + "}", false);
        }

        /// <summary>
        /// Repeats the previous item between <paramref name="n"/> and <paramref name="m"/> times (<c>{n,m}</c>).
        /// </summary>
        public VerbalExpressions RepeatPrevious(int n, int m)
        {
            return Add("{" + n + "," + m + "}", false);
        }

        /// <summary>
        /// Same as <see cref="RepeatPrevious(int)"/>.
        /// </summary>
        public VerbalExpressions Count(int n)
        {
            return RepeatPrevious(n);
        }

        /// <summary>
        /// Same as <see cref="RepeatPrevious(int, int)"/>.
        /// </summary>
        public VerbalExpressions Count(int min, int max)
        {
            return RepeatPrevious(min, max);
        }

        /// <summary>
        /// Repeats the previous item at least <paramref name="n"/> times (<c>{n,}</c>).
        /// </summary>
        public VerbalExpressions AtLeast(int n)
        {
            ValidateCount(n, "n");
            return Add("{" + n + ",}", false);
        }

        /// <summary>
        /// Repeats the previous item one or more times (<c>+</c>).
        /// </summary>
        public VerbalExpressions OneOrMore()
        {
            return Add("+", false);
        }

        /// <summary>
        /// Repeats the previous item zero or more times (<c>*</c>).
        /// </summary>
        public VerbalExpressions ZeroOrMore()
        {
            return Add("*", false);
        }

        #endregion Expression Modifiers

        #region Character Classes

        /// <summary>
        /// Matches a single ASCII letter (<c>[a-zA-Z]</c>).
        /// </summary>
        public VerbalExpressions Letter()
        {
            return Add("[a-zA-Z]", false);
        }

        /// <summary>
        /// Matches a single upper-case ASCII letter (<c>[A-Z]</c>).
        /// </summary>
        public VerbalExpressions UpperCaseLetter()
        {
            return Add("[A-Z]", false);
        }

        /// <summary>
        /// Matches a single lower-case ASCII letter (<c>[a-z]</c>).
        /// </summary>
        public VerbalExpressions LowerCaseLetter()
        {
            return Add("[a-z]", false);
        }

        /// <summary>
        /// Matches a single digit (<c>\d</c>).
        /// </summary>
        public VerbalExpressions Digit()
        {
            return Add(@"\d", false);
        }

        /// <summary>
        /// Matches a single character that is not a digit (<c>\D</c>).
        /// </summary>
        public VerbalExpressions NonDigit()
        {
            return Add(@"\D", false);
        }

        /// <summary>
        /// Matches a single word character: letter, digit or underscore (<c>\w</c>).
        /// </summary>
        public VerbalExpressions WordChar()
        {
            return Add(@"\w", false);
        }

        /// <summary>
        /// Matches a single character that is not a word character (<c>\W</c>).
        /// </summary>
        public VerbalExpressions NonWordChar()
        {
            return Add(@"\W", false);
        }

        /// <summary>
        /// Matches a single whitespace character (<c>\s</c>).
        /// </summary>
        public VerbalExpressions Whitespace()
        {
            return Add(@"\s", false);
        }

        /// <summary>
        /// Matches a single character that is not whitespace (<c>\S</c>).
        /// </summary>
        public VerbalExpressions NonWhitespace()
        {
            return Add(@"\S", false);
        }

        /// <summary>
        /// Same as <see cref="Whitespace"/>. Matches the Java API name.
        /// </summary>
        public VerbalExpressions Space()
        {
            return Whitespace();
        }

        /// <summary>
        /// Same as <see cref="NonWhitespace"/>. Matches the Java API name.
        /// </summary>
        public VerbalExpressions NonSpace()
        {
            return NonWhitespace();
        }

        /// <summary>
        /// Matches a word boundary (<c>\b</c>).
        /// </summary>
        public VerbalExpressions WordBoundary()
        {
            return Add(@"\b", false);
        }

        #endregion Character Classes

        #region Lookaround

        /// <summary>
        /// Asserts that <paramref name="value"/> follows, without consuming it (<c>(?=...)</c>).
        /// </summary>
        public VerbalExpressions LookAhead(string value, bool sanitize = true)
        {
            value = sanitize ? Sanitize(value) : value;
            return Add("(?=" + value + ")", false);
        }

        /// <summary>
        /// Asserts that <paramref name="value"/> does not follow (<c>(?!...)</c>).
        /// </summary>
        public VerbalExpressions NegativeLookAhead(string value, bool sanitize = true)
        {
            value = sanitize ? Sanitize(value) : value;
            return Add("(?!" + value + ")", false);
        }

        /// <summary>
        /// Same as <see cref="NegativeLookAhead"/>. Matches the JavaScript API name.
        /// </summary>
        public VerbalExpressions Not(string value, bool sanitize = true)
        {
            return NegativeLookAhead(value, sanitize);
        }

        /// <summary>
        /// Asserts that <paramref name="value"/> comes just before, without consuming it (<c>(?&lt;=...)</c>).
        /// </summary>
        public VerbalExpressions LookBehind(string value, bool sanitize = true)
        {
            value = sanitize ? Sanitize(value) : value;
            return Add("(?<=" + value + ")", false);
        }

        /// <summary>
        /// Asserts that <paramref name="value"/> does not come just before (<c>(?&lt;!...)</c>).
        /// </summary>
        public VerbalExpressions NegativeLookBehind(string value, bool sanitize = true)
        {
            value = sanitize ? Sanitize(value) : value;
            return Add("(?<!" + value + ")", false);
        }

        #endregion Lookaround

        #region Expression Options Modifiers

        /// <summary>
        /// Turns on a regex option by its letter: <c>i</c> (ignore case),
        /// <c>x</c> (ignore pattern whitespace), <c>m</c> (multiline) or <c>s</c> (single line).
        /// Other letters are ignored.
        /// </summary>
        public VerbalExpressions AddModifier(char modifier)
        {
            switch (modifier)
            {
                case 'i':
                    _modifiers |= RegexOptions.IgnoreCase;
                    break;
                case 'x':
                    _modifiers |= RegexOptions.IgnorePatternWhitespace;
                    break;
                case 'm':
                    _modifiers |= RegexOptions.Multiline;
                    break;
                case 's':
                    _modifiers |= RegexOptions.Singleline;
                    break;
            }

            return this;
        }

        /// <summary>
        /// Turns off a regex option by its letter. See <see cref="AddModifier"/>.
        /// </summary>
        public VerbalExpressions RemoveModifier(char modifier)
        {
            switch (modifier)
            {
                case 'i':
                    _modifiers &= ~RegexOptions.IgnoreCase;
                    break;
                case 'x':
                    _modifiers &= ~RegexOptions.IgnorePatternWhitespace;
                    break;
                case 'm':
                    _modifiers &= ~RegexOptions.Multiline;
                    break;
                case 's':
                    _modifiers &= ~RegexOptions.Singleline;
                    break;
            }

            return this;
        }

        /// <summary>
        /// Makes matching case-insensitive (or case-sensitive again with <c>false</c>).
        /// </summary>
        public VerbalExpressions WithAnyCase(bool enable = true)
        {
            if (enable)
            {
                AddModifier('i');
            }
            else
            {
                RemoveModifier('i');
            }
            return this;
        }

        /// <summary>
        /// If true, <c>^</c> and <c>$</c> match only at the start and end of the whole
        /// input instead of at every line. Same as <see cref="SearchOneLine"/>.
        /// </summary>
        public VerbalExpressions UseOneLineSearchOption(bool enable)
        {
            if (enable)
            {
                RemoveModifier('m');
            }
            else
            {
                AddModifier('m');
            }

            return this;
        }

        /// <summary>
        /// If true (the default), <c>^</c> and <c>$</c> match only at the start and end of the
        /// whole input instead of at every line. Matches the Java and JavaScript API name.
        /// </summary>
        public VerbalExpressions SearchOneLine(bool enable = true)
        {
            return UseOneLineSearchOption(enable);
        }

        /// <summary>
        /// Replaces all regex options with <paramref name="options"/>.
        /// </summary>
        public VerbalExpressions WithOptions(RegexOptions options)
        {
            this._modifiers = options;
            return this;
        }

        #endregion Expression Options Modifiers

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Escapes a value for use inside a character set. <see cref="Regex.Escape"/>
        /// does not escape <c>]</c>, which would otherwise close the set early.
        /// </summary>
        private string SanitizeCharacterSet(string value)
        {
            return Sanitize(value).Replace("]", @"\]");
        }

        private static void ValidateCount(int value, string paramName)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(paramName, "Count cannot be negative");
            }
        }

        #endregion Private Methods
    }
}
