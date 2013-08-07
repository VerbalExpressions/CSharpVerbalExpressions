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
        public static VerbalExpressions NewExpression
        {
            get
            {
                return new VerbalExpressions();
            }
        }
        #endregion Statics

        #region Private Members
        private string _prefixes = "";
        private string _source = "";
        private string _suffixes = "";
        private RegexOptions _modifiers = RegexOptions.Multiline;
        #endregion Private Members

        #region Private Properties
        private string RegexString
        {
            get
            {
                return _prefixes + _source + _suffixes;
            }
        }
        private Regex PatternRegex
        {
            get
            {
                return new Regex(this.RegexString, _modifiers);
            }
        }
        #endregion Private Properties

        #region Constructors
        private VerbalExpressions()
        {
        }
        static VerbalExpressions()
        {
        }
        #endregion Constructors

        #region Public Methods
        public string Sanitize(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value must be provided");
            }

            return Regex.Escape(value);
        }
      
        public VerbalExpressions Add(CommonRegex commonRegex)
        {
            return Add(commonRegex.ToString(), false);
        }

        public VerbalExpressions Add(string value, bool sanitize = true)
        {
            if (value == null)
                throw new ArgumentNullException("value must be provided");

            value = sanitize ? Sanitize(value) : value;
            _source += value;
            return this;
        }

        public VerbalExpressions StartOfLine(bool enable = true)
        {
            _prefixes = enable ? "^" : string.Empty;
            return this;
        }

        public VerbalExpressions EndOfLine(bool enable = true)
        {
            _suffixes = enable ? "$" : string.Empty;
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
            return Then(commonRegex.ToString(), false);
        }

        public VerbalExpressions Find(string value)
        {
            return Then(value);
        }

        public VerbalExpressions Maybe(string value, bool sanitize = true)
        {
            value = sanitize ? Sanitize(value) : value;
            value = string.Format("({0})?", Sanitize(value));
            return Add(value, false);
        }

        public VerbalExpressions Maybe(CommonRegex commonRegex)
        {
            return Maybe(commonRegex.ToString(), sanitize: false);
        }

        public VerbalExpressions Anything()
        {
            return Add("(.*)",false);
        }

        public VerbalExpressions AnythingBut(string value, bool sanitize = true)
        {
            value = sanitize ? Sanitize(value) : value;
            value = string.Format("([^{0}]*)", value);
            return Add(value,false);
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

        public VerbalExpressions AnyOf(string value)
        {
            value = string.Format("[{0}]",Sanitize(value));
            return Add(value, false);
        }

        public VerbalExpressions Any(string value)
        {
            return AnyOf(value);
        }

        public VerbalExpressions Range(params object[] args)
        {
            if (object.ReferenceEquals(args, null))
            {
                throw new ArgumentNullException("args parameter must not be null");
            }

            if (args.Length == 1)
            {
                throw new ArgumentOutOfRangeException("range must have at least 2 values");
            }
            
            string[] sanitizedStrings = args.Select(argument =>
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
                .Where(sanitizedString => !string.IsNullOrEmpty(sanitizedString))
                .OrderBy(s => s)
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
                sb.AppendFormat("{0}-{1}", sanitizedStrings[_from], sanitizedStrings[_to]);
            }
            sb.Append("]");

            if (hasOddNumberOfParams)
            {
                sb.AppendFormat("|{0}", sanitizedStrings.Last());
            }

            return Add(sb.ToString(),false);
        }

        public VerbalExpressions AddModifier(char modifier)
        {
            switch (modifier)
            {
                //case 'd':
                //	_modifiers |= RegexOptions.UNIX_LINES;
                //	break;
                case 'i':
                    _modifiers |= RegexOptions.IgnoreCase;
                    break;
                case 'x':
                    _modifiers |= RegexOptions.IgnorePatternWhitespace;
                    break;
                case 'm':
                    _modifiers |= RegexOptions.Multiline;
                    break;
                //case 's':
                //	_modifiers |= RegexOptions.DOTALL;
                //	break;
                //case 'u':
                //	_modifiers |= Pattern.UNICODE_CASE;
                //	break;
                //case 'U':
                //	_modifiers |= Pattern.UNICODE_CHARACTER_CLASS;
                //	break;
            }

            return this;
        }

        public VerbalExpressions RemoveModifier(char modifier)
        {
            switch (modifier)
            {
                //case 'd':
                //	_modifiers &= ~Pattern.UNIX_LINES;
                //	break;
                case 'i':
                    _modifiers &= ~RegexOptions.IgnoreCase;
                    break;
                case 'x':
                    _modifiers &= ~RegexOptions.IgnorePatternWhitespace;
                    break;
                case 'm':
                    _modifiers &= ~RegexOptions.Multiline;
                    break;
                //case 's':
                //	_modifiers &= ~Pattern.DOTALL;
                //	break;
                //case 'u':
                //	_modifiers &= ~Pattern.UNICODE_CASE;
                //	break;
                //case 'U':
                //	_modifiers &= ~Pattern.UNICODE_CHARACTER_CLASS;
                //	break;
                //default:
                //	break;
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

        public VerbalExpressions SearchOneLine(bool enable)
        {
            if (enable)
                RemoveModifier('m');
            else
                AddModifier('m');
            return this;
        }

        public VerbalExpressions Multiple(string value)
        {
            value = Sanitize(value);
            switch (value[0])
            {
                case '*':
                case '+':
                    break;
                default:
                    value += '+';
                    break;
            }


            return Add(value, false);
        }

        public VerbalExpressions Or(CommonRegex commonRegex)
        {
            return Or(commonRegex.ToString(),false);
        }

        public VerbalExpressions Or(string value, bool sanitize = true)
        {
            if (_prefixes.IndexOf("(") == -1)
                _prefixes += "(";
            if (_suffixes.IndexOf(")") == -1)
                _suffixes = ")" + _suffixes;

            _source += ")|(";

            return Add(value, sanitize);
        }

        public VerbalExpressions WithOptions(RegexOptions options)
        {
            this._modifiers = options;
            return this;
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
        #endregion Public Methods
    }
}
