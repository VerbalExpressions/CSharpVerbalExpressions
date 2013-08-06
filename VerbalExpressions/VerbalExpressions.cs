/*!
 * CSharpVerbalExpressions v0.1
 * https://github.com/VerbalExpressions/CSharpVerbalExpressions
 * 
 * @psoholt
 *
 * Date: 2013-07-26
 * 
 */

using System.Text.RegularExpressions;

namespace VerbalExpression.Net
{
    public class VerbalExpressions
    {
        private string _prefixes = "";
        private string _source = "";
        private string _suffixes = "";

        private RegexOptions _modifiers = RegexOptions.Multiline;
        private Regex patternRegex;

        private string Sanitize(string value)
        {
            if (value == null)
                return string.Empty;
            return Regex.Escape(value);
        }
        
        public VerbalExpressions Add(CommonRegex commonRegex)
        {
            return Add(commonRegex.ToString(), sanitize: false);
        }

        public VerbalExpressions Add(string value, bool sanitize = true)
        {
            value = sanitize ? Sanitize(value) : value;
            _source = _source != null ? _source + value : value;
            if (_source != null)
                patternRegex = new Regex(_prefixes + _source + _suffixes, _modifiers);

            return this;
        }

        public VerbalExpressions StartOfLine(bool enable = true)
        {
            _prefixes = enable ? "^" : "";
            return this;
        }

        public VerbalExpressions EndOfLine(bool enable = true)
        {
            _suffixes = enable ? "$" : "";
            return this;
        }

        public VerbalExpressions Then(string value, bool sanitize = true)
        {
            value = sanitize ? Sanitize(value) : value;
            Add("(" + value + ")", sanitize: false);
            return this;
        }

        public VerbalExpressions Then(CommonRegex commonRegex)
        {
            return Then(commonRegex.ToString(), sanitize: false);
        }

        public VerbalExpressions Find(string value)
        {
            Then(value);
            return this;
        }

        public VerbalExpressions Maybe(string value, bool sanitize = true)
        {
            value = sanitize ? Sanitize(value) : value;
            Add("(" + value + ")?", sanitize: false);
            return this;
        }

        public VerbalExpressions Maybe(CommonRegex commonRegex)
        {
            return Maybe(commonRegex.ToString(), sanitize: false);
        }

        public VerbalExpressions Anything()
        {
            Add("(.*)", sanitize: false);
            return this;
        }

        public VerbalExpressions AnythingBut(string value, bool sanitize = true)
        {
            value = sanitize ? Sanitize(value) : value;
            Add("([^" + value + "]*)", sanitize: false);
            return this;
        }

        public VerbalExpressions Replace(string value)
        {
            _source.Replace(patternRegex.ToString(), value);
            return this;
        }

        public VerbalExpressions LineBreak()
        {
            Add("(\\n|(\\r\\n))", sanitize: false);
            return this;
        }

        public VerbalExpressions Br()
        {
            LineBreak();
            return this;
        }

        public VerbalExpressions Tab()
        {
            Add("\\t");
            return this;
        }

        public VerbalExpressions Word()
        {
            Add("\\w+", sanitize: false);
            return this;
        }

        public VerbalExpressions AnyOf(string value)
        {
            value = Sanitize(value);
            Add("[" + value + "]", sanitize: false);
            return this;
        }

        public VerbalExpressions Any(string value)
        {
            AnyOf(value);
            return this;
        }

        public VerbalExpressions Range(object[] args)
        {
            string value = "[";
            for (int _from = 0; _from < args.Length; _from += 2)
            {
                int _to = _from + 1;
                if (args.Length <= _to) break;
                string from = Sanitize((string)args[_from]);
                string to = Sanitize((string)args[_to]);

                value += from + "-" + to;
            }

            value += "]";

            Add(value, sanitize: false);
            return this;
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

        public VerbalExpressions SearchOneLine(bool enable = true)
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

            Add(value, sanitize: false);
            return this;
        }

        public VerbalExpressions Or(CommonRegex commonRegex)
        {
            Or(commonRegex.ToString(), sanitize: false);
            return this;
        }

        public VerbalExpressions Or(string value, bool sanitize = true)
        {
            if (_prefixes.IndexOf("(") == -1)
                _prefixes += "(";
            if (_suffixes.IndexOf(")") == -1)
                _suffixes = ")" + _suffixes;

            Add(")|(", sanitize: false);
            if (value != null)
                Add(value, sanitize: false);
            return this;
        }

        public bool Test(string toTest)
        {
            return IsMatch(toTest);
        }

        public bool IsMatch(string toTest)
        {
            Add(string.Empty);
            return patternRegex.IsMatch(toTest);
        }

        public Regex ToRegex()
        {
            Add(string.Empty);
            return patternRegex;
        }

        public override string ToString()
        {
            Add(string.Empty);
            return patternRegex.ToString();
        }
    }
}
