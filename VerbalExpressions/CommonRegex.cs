namespace CSharpVerbalExpressions
{
    /// <summary>
    ///     This class is used to fake an enum. You'll be able to use it as an enum.
    ///     Note: type save enum, found on stackoverflow: http://stackoverflow.com/a/424414/603309
    /// </summary>
    public sealed class CommonRegex
    {
        #region Private Members

        #endregion Private Members

        #region Public Properties

        public string Name { get; }

        public int Value { get; }

        #endregion Public Properties

        #region Statics

        public static readonly CommonRegex Url = new CommonRegex(1,
                @"((([A-Za-z]{3,9}:(?:\/\/)?)(?:[^-;:&=\+\$,\w]+@)?[A-Za-z0-9.-]+(:[0-9]+)?|(?:www.|[^-;:&=\+\$,\w]+@)[A-Za-z0-9.-]+)((?:\/[\+~%\/.\w_]*)?\??(?:[-\+=&;%@.\w-_]*)#?‌​(?:[\w]*))?)")
            ;

        public static readonly CommonRegex Email =
            new CommonRegex(2, @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}");

        #endregion Statics

        #region Constructors

        private CommonRegex(int value, string name)
        {
            Name = name;
            Value = value;
        }

        static CommonRegex()
        {
        }

        #endregion Constructors
    }
}