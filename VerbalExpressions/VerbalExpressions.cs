using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CSharpVerbalExpressions
{
    public class VerbalExpressions
    {
        #region Statics

        /// <summary>
        ///     Returns a default instance of VerbalExpressions
        ///     having the Multiline option enabled
        /// </summary>
        public static VerbalExpressions DefaultExpression => new VerbalExpressions();

        #endregion Statics

        #region Private Members

        private readonly RegexCache regexCache = new RegexCache();
        private readonly StringBuilder _prefixes = new StringBuilder();
        private readonly StringBuilder _source = new StringBuilder();
        private readonly StringBuilder _suffixes = new StringBuilder();

        private RegexOptions _modifiers = RegexOptions.Multiline;

        private bool isPreviousExpressionAnythingBut;

        #endregion Private Members

        #region Private Properties

        private string RegexString =>
            new StringBuilder().Append(_prefixes).Append(_source).Append(_suffixes).ToString();

        private Regex PatternRegex => regexCache.Get(RegexString, _modifiers);

        #endregion Private Properties

        #region Public Methods

        #region Helpers

        public string Sanitize(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

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

        public VerbalExpressions Add(string value)
        {
            if (ReferenceEquals(value, null))
                throw new ArgumentNullException("value");

            return Add(value, true);
        }

        public VerbalExpressions Add(CommonRegex commonRegex)
        {
            return Add(commonRegex.Name, false);
        }

        public VerbalExpressions Add(string value, bool sanitize = true)
        {
            if (value == null)
                throw new ArgumentNullException("value must be provided");

            if (isPreviousExpressionAnythingBut) return this;

            value = sanitize ? Sanitize(value) : value;
            _source.Append(value);
            return this;
        }

        public VerbalExpressions StartOfLine(bool enable = true)
        {
            _prefixes.Append(enable ? "^" : string.Empty);
            return this;
        }

        public VerbalExpressions EndOfLine(bool enable = true)
        {
            _suffixes.Append(enable ? "$" : string.Empty);
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
            return Maybe(commonRegex.Name, false);
        }

        public VerbalExpressions Anything()
        {
            return Add("(.*)", false);
        }

        public VerbalExpressions AnythingBut(string value, bool sanitize = true)
        {
            value = sanitize ? Sanitize(value) : value;
            value = string.Format("([^{0}]*)", value);
            isPreviousExpressionAnythingBut = true;
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
            var whereToReplace = PatternRegex.ToString();

            if (whereToReplace.Length != 0)
                _source.Replace(whereToReplace, value);

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
                throw new ArgumentNullException("value");

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
            if (ReferenceEquals(arguments, null))
                throw new ArgumentNullException("arguments");

            if (arguments.Length == 1)
                throw new ArgumentOutOfRangeException("arguments");

            var sanitizedStrings = arguments.Select(argument =>
                {
                    if (ReferenceEquals(argument, null))
                        return string.Empty;

                    var casted = argument.ToString();
                    if (string.IsNullOrEmpty(casted))
                        return string.Empty;
                    return Sanitize(casted);
                })
                .Where(sanitizedString =>
                    !string.IsNullOrEmpty(sanitizedString))
                .OrderBy(s => s)
                .ToArray();

            if (sanitizedStrings.Length > 3)
                throw new ArgumentOutOfRangeException("arguments");

            if (!sanitizedStrings.Any())
                return this;

            var hasOddNumberOfParams = sanitizedStrings.Length % 2 > 0;

            var sb = new StringBuilder("[");
            for (var _from = 0; _from < sanitizedStrings.Length; _from += 2)
            {
                var _to = _from + 1;
                if (sanitizedStrings.Length <= _to)
                    break;
                sb.AppendFormat("{0}-{1}", sanitizedStrings[_from], sanitizedStrings[_to]);
            }
            sb.Append("]");

            if (hasOddNumberOfParams)
                sb.AppendFormat("|{0}", sanitizedStrings.Last());

            return Add(sb.ToString(), false);
        }

        public VerbalExpressions Multiple(string value, bool sanitize = true)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            value = sanitize ? Sanitize(value) : value;
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
                AddModifier('i');
            else
                RemoveModifier('i');
            return this;
        }

        public VerbalExpressions UseOneLineSearchOption(bool enable)
        {
            if (enable)
                RemoveModifier('m');
            else
                AddModifier('m');

            return this;
        }

        public VerbalExpressions WithOptions(RegexOptions options)
        {
            _modifiers = options;
            return this;
        }

        #endregion Expression Options Modifiers

        #endregion Public Methods
    }
}