/*!
 * CSharpVerbalExpressions v0.1
 * https://github.com/VerbalExpressions/CSharpVerbalExpressions
 * 
 * @psoholt
 *
 * Date: 2013-07-26
 * 
 */

using System.Text;
using System.Text.RegularExpressions;

namespace VerbalExpression.Net
{
	public class VerbalExpressions
	{
		private string _prefixes = null;
		private string _source = null;
		private string _suffixes = null;

        private RegexOptions _options = RegexOptions.Multiline;

        public VerbalExpressions()
        {
            
        }

        public VerbalExpressions(RegexOptions options)
        {
            _options = options;
        }

		private static string Sanitize(string value)
		{
			if (value == null)
				return value;

			return Regex.Escape(value);
		}

        private static string Sanitize(VerbalExpressions verbEx)
        {
            if (verbEx == null)
                return null;

            return verbEx._source;
        }

        public VerbalExpressions Add(string value)
		{
			_source += value;
			return this;
		}

		public VerbalExpressions StartOfLine(bool enable = true)
		{
			_prefixes = enable ? "^" : null;
			return this;
		}

		public VerbalExpressions EndOfLine(bool enable = true)
		{
			_suffixes = enable ? "$" : null;
			return this;
		}

		public VerbalExpressions Then(string value)
		{
			value = Sanitize(value);
			Add("(" + value + ")");
			return this;
		}

        public VerbalExpressions Then(VerbalExpressions verbEx)
        {
            Add("(" + Sanitize(verbEx) + ")");
            return this;
        }

        public VerbalExpressions Find(string value)
		{
            return Then(value);
		}

        public VerbalExpressions Find(VerbalExpressions verbEx)
        {
            return Then(verbEx);
        }

		public VerbalExpressions Maybe(string value)
		{
			value = Sanitize(value);
			Add("(" + value + ")?");
			return this;
		}

        public VerbalExpressions Maybe(VerbalExpressions verbEx)
        {
            Add("(" + Sanitize(verbEx) + ")?");
            return this;
        }

		public VerbalExpressions Anything()
		{
			Add("(.*)");
			return this;
		}

		public VerbalExpressions AnythingBut(string value)
		{
			value = Sanitize(value);
			Add("([^" + value + "]*)");
			return this;
		}

        public VerbalExpressions AnythingBut(VerbalExpressions verbEx)
        {
            Add("([^" + Sanitize(verbEx) + "]*)");
            return this;
        }

        public VerbalExpressions Something()
		{
			Add("(.+)");
			return this;
		}

        public VerbalExpressions SomethingBut(string value)
        {
            value = Sanitize(value);
            Add("([^" + value + "+)");
            return this;
        }

        public VerbalExpressions SomethingBut(VerbalExpressions verbEx)
        {
            Add("([^" + Sanitize(verbEx) + "]+)");
            return this;
        }

        public string Replace(string input, string replacement)
		{
            string replaced = this.ToRegex().Replace(input, replacement);
            return replaced;
		}

		public VerbalExpressions LineBreak()
		{
			Add(@"(\n|(\r\n))");
			return this;
		}

		public VerbalExpressions Br()
		{
			LineBreak();
			return this;
		}

		public VerbalExpressions Tab()
		{
			Add(@"\t");
			return this;
		}

		public VerbalExpressions Word()
		{
			Add(@"\w+");
			return this;
		}

		public VerbalExpressions AnyOf(string value)
		{
			value = Sanitize(value);
			Add("[" + value + "]");
			return this;
		}

        public VerbalExpressions AnyOf(VerbalExpressions verbEx)
        {
            Add("[" + Sanitize(verbEx) + "]");
            return this;
        }

        public VerbalExpressions Any(string value)
		{
			return AnyOf(value);
		}

        public VerbalExpressions Any(VerbalExpressions verbEx)
        {
            return AnyOf(verbEx);
        }

		public VerbalExpressions Range(params string[] args)
		{
            StringBuilder sb = new StringBuilder("[");

			for (int _from = 0; _from < args.Length; _from += 2)
			{
				int _to = _from + 1;
				if (args.Length <= _to)
                    break;

				string from = Sanitize(args[_from]);
				string to = Sanitize(args[_to]);

                sb.Append(from);
                sb.Append("-");
                sb.Append(to);
			}

			sb.Append("]");

			Add(sb.ToString());
			return this;
		}

        public VerbalExpressions Range(params VerbalExpressions[] args)
        {
            StringBuilder sb = new StringBuilder("[");

            for (int _from = 0; _from < args.Length; _from += 2)
            {
                int _to = _from + 1;
                if (args.Length <= _to)
                    break;

                string from = Sanitize(args[_from]);
                string to = Sanitize(args[_to]);

                sb.Append(from);
                sb.Append("-");
                sb.Append(to);
            }

            sb.Append("]");

            Add(sb.ToString());
            return this;
        }
        
        public VerbalExpressions AddModifier(char modifier)
		{
			switch (modifier)
			{
				//case 'd':
				//_modifiers |= RegexOptions.UNIX_LINES;
				//	break;
				case 'i':
                    _options |= RegexOptions.IgnoreCase;
					break;
				case 'x':
					_options |= RegexOptions.IgnorePatternWhitespace;
					break;
				case 'm':
					_options |= RegexOptions.Multiline;
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
					_options &= ~RegexOptions.IgnoreCase;
					break;
				case 'x':
					_options &= ~RegexOptions.IgnorePatternWhitespace;
					break;
				case 'm':
					_options &= ~RegexOptions.Multiline;
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
            value += '+';

            Add(value);
            
            return this;
		}

        public VerbalExpressions Multiple(VerbalExpressions verbEx)
        {
            string value = Sanitize(verbEx);

            if (!string.IsNullOrEmpty(value))
            {
                // Add a plus if the last character is not already
                // a * or +
                switch (value[value.Length - 1])
                {
                    case '*':
                    case '+':
                        break;
                    default:
                        value += '+';
                        break;
                }

                Add(value);
            }

            return this;
        }

        public VerbalExpressions Or(string value)
        {
            _prefixes += "(";
            _suffixes = ")" + _suffixes;

            Add(")|(");

            if (value != null)
                Then(value);

            return this;
        }

        public VerbalExpressions Or(VerbalExpressions verbEx)
        {
            _prefixes += "(";
            _suffixes = ")" + _suffixes;

            Add(")|(");

            if (verbEx != null)
                Then(verbEx);

            return this;
        }

        public bool Test(string toTest)
		{
			return IsMatch(toTest);
		}

		public bool IsMatch(string toTest)
		{
			return this.ToRegex().IsMatch(toTest);
		}

		public Regex ToRegex()
		{
			Regex regex = new Regex(this.ToString(), _options);
			return regex;
		}

		public override string ToString()
		{
            return _prefixes + _source + _suffixes;
		}

        public static implicit operator Regex(VerbalExpressions verbEx)
        {
            if (verbEx == null)
                return null;
            
            return verbEx.ToRegex();
        }

        public static explicit operator string(VerbalExpressions verbEx)
        {
            if (verbEx == null)
                return null;

            return verbEx.ToString();
        }
	}
}
