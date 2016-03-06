using System;
using System.Reflection;
using NUnitLite;

namespace VerbalExpressionsUnitTests
{
    public class Program
    {
        public static void Main(string[] args)
        {

            new AutoRun().Execute(typeof(Program).GetTypeInfo().Assembly, Console.Out, Console.In, args);

        }
    }
}