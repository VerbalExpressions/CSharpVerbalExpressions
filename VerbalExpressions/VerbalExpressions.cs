/*!
 * CSharpVerbalExpressions v0.1
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
    public class VerbalExpressions
    {
        #region Statics

        /// <summary>
        /// Returns a default instance of VerbalExpressions
        /// having the Multiline option enabled
        /// </summary>
        public static VerbalExpressions DefaultExpression
        {
            get { return new VerbalExpressions(); }
        }

        #endregion Statics

        #region Private Members

        private readonly RegexCache regexCache = new RegexCache();
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

        public string Sanitize(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            return Regex.Escape(value);
        }

        public bool Test(string toTest)
        {
            return IsMatch(toTest);
        }

        public bool IsMatch(string toTest)
        {
            return PatternRegex.IsMatch(toTest);
        }

        public Regex ToRegex()
        {
            return PatternRegex;
        }

        public override string ToString()
        {
            return PatternRegex.ToString();
        }

        public string Capture(string toTest, string groupName)
        {
            if (!Test(toTest))
                return null;

            var match = PatternRegex.Match(toTest);
            return match.Groups[groupName].Value;
        }

        #endregion Helpers

        #region Expression Modifiers

        public VerbalExpressions Add(CommonRegex commonRegex)
        {
            return Add(commonRegex.Name, false);
        }

        public VerbalExpressions Add(string value, bool sanitize = true)
        {
            if (value == null)
                throw new ArgumentNullException("value must be provided");

            value = sanitize ? Sanitize(value) : value;
            _source.Append(value);
            return this;
        }

        public VerbalExpressions StartOfLine(bool enable = true)
        {
            _prefixes.Append(enable ? "^" : String.Empty);
            return this;
        }

        public VerbalExpressions EndOfLine(bool enable = true)
        {
            _suffixes.Append(enable ? "$" : String.Empty);
            return this;
        }

        public VerbalExpressions Then(string value, bool sanitize = true)
        {
            var sanitizedValue = sanitize ? Sanitize(value) : value;
            value = string.Format("({0})", sanitizedValue);
            return Add(value, false);
        }

        public VerbalExpressions Then(CommonRegex commonRegex)
        {
            return Then(commonRegex.Name, false);
        }

        public VerbalExpressions Find(string value)
        {
            return Then(value);
        }

        public VerbalExpressions Maybe(string value, bool sanitize = true)
        {
            value = sanitize ? Sanitize(value) : value;
            value = string.Format("({0})?", value);
            return Add(value, false);
        }

        public VerbalExpressions Maybe(CommonRegex commonRegex)
        {
            return Maybe(commonRegex.Name, sanitize: false);
        }

        public VerbalExpressions Anything()
        {
            return Add("(.*)", false);
        }

        public VerbalExpressions AnythingBut(string value, bool sanitize = true)
        {
            value = sanitize ? Sanitize(value) : value;
            value = string.Format("([^{0}]*)", value);
            return Add(value, false);
        }

        public VerbalExpressions Something()
        {
            return Add("(.+)", false);
        }

        public VerbalExpressions SomethingBut(string value, bool sanitize = true)
        {
            value = sanitize ? Sanitize(value) : value;
            value = string.Format("([^" + value + "]+)");
            return Add(value, false);
        }

        public VerbalExpressions Replace(string value)
        {
            string whereToReplace = PatternRegex.ToString();

            if (whereToReplace.Length != 0)
            {
                _source.Replace(whereToReplace, value);
            }

            return this;
        }

        public VerbalExpressions LineBreak()
        {
            return Add(@"(\n|(\r\n))", false);
        }

        public VerbalExpressions Br()
        {
            return LineBreak();
        }

        public VerbalExpressions Tab()
        {
            return Add(@"\t");
        }

        public VerbalExpressions Word()
        {
            return Add(@"\w+", false);
        }

        public VerbalExpressions AnyOf(string value, bool sanitize = true)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            value = sanitize ? Sanitize(value) : value;
            value = string.Format("[{0}]", value);
            return Add(value, false);
        }

        public VerbalExpressions Any(string value)
        {
            return AnyOf(value);
        }

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
                .OrderBy(s => s)
                .ToArray();

            if (sanitizedStrings.Length > 3)
            {
                throw new ArgumentOutOfRangeException("arguments");
            }

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
                sb.AppendFormat("{0}-{1}", sanitizedStrings[_from], sanitizedStrings[_to]);
            }
            sb.Append("]");

            if (hasOddNumberOfParams)
            {
                sb.AppendFormat("|{0}", sanitizedStrings.Last());
            }

            return Add(sb.ToString(), false);
        }

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

        public VerbalExpressions Or(CommonRegex commonRegex)
        {
            return Or(commonRegex.Name, false);
        }

        public VerbalExpressions Or(string value, bool sanitize = true)
        {
            _prefixes.Append("(");
            _suffixes.Insert(0, ")");
            _source.Append(")|(");
            return Add(value, sanitize);
        }

        public VerbalExpressions BeginCapture()
        {
            return Add("(", false);
        }

        public VerbalExpressions BeginCapture(string groupName)
        {
            return Add("(?<", false).Add(groupName, true).Add(">", false);
        }

        public VerbalExpressions EndCapture()
        {
            return Add(")", false);
        }

        public VerbalExpressions RepeatPrevious(int n)
        {
            return Add("{" + n + "}", false);
        }

        public VerbalExpressions RepeatPrevious(int n, int m)
        {
            return Add("{" + n + "," + m + "}", false);
        }

        #endregion Expression Modifiers

        #region Expression Options Modifiers

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

        public VerbalExpressions WithOptions(RegexOptions options)
        {
            this._modifiers = options;
            return this;
        }

        #endregion Expression Options Modifiers

        #endregion Public Methods


    }
}
