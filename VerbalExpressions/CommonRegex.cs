using System;

namespace CSharpVerbalExpressions
{

    /// <summary>
    /// This class is used to fake an enum. You'll be able to use it as an enum.
    /// Note: type save enum, found on stackoverflow: http://stackoverflow.com/a/424414/603309
    /// </summary>
    public sealed class CommonRegex
    {

        private readonly String name;
        private readonly int value;

        public static readonly CommonRegex Url = new CommonRegex(1, @"((([A-Za-z]{3,9}:(?:\/\/)?)(?:[^-;:&=\+\$,\w]+@)?[A-Za-z0-9.-]+(:[0-9]+)?|(?:ww‌​w.|[^-;:&=\+\$,\w]+@)[A-Za-z0-9.-]+)((?:\/[\+~%\/.\w-_]*)?\??(?:[-\+=&;%@.\w_]*)#?‌​(?:[\w]*))?)");
        public static readonly CommonRegex Email = new CommonRegex(2, @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}");
        
        private CommonRegex(int value, String name)
        {
            this.name = name;
            this.value = value;
        }

        public int ToSwitchableValue()
        {
            return value;
        }

        public override String ToString()
        {
            return name;
        }

    }
}
