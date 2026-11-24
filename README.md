# CSharpVerbalExpressions

[![CI](https://github.com/VerbalExpressions/CSharpVerbalExpressions/actions/workflows/ci.yml/badge.svg)](https://github.com/VerbalExpressions/CSharpVerbalExpressions/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/VerbalExpressions-official.svg)](https://www.nuget.org/packages/VerbalExpressions-official)
[![License](https://img.shields.io/github/license/VerbalExpressions/CSharpVerbalExpressions.svg)](https://github.com/VerbalExpressions/CSharpVerbalExpressions/blob/master/LICENSE)
[![Ported from JSVerbalExpressions](https://img.shields.io/badge/ported%20from-JSVerbalExpressions-orange.svg)](https://github.com/VerbalExpressions/JSVerbalExpressions)

## C# Regular Expressions made easy

VerbalExpressions is a C# library that helps you build hard-to-read regular expressions with a readable, fluent API.

```csharp
var verbEx = new VerbalExpressions()
    .StartOfLine()
    .Then("http")
    .Maybe("s")
    .Then("://")
    .Maybe("www.")
    .AnythingBut(" ")
    .EndOfLine();

verbEx.Test("https://www.google.com"); // true
verbEx.ToString();                     // ^(http)(s)?(://)(www\.)?([^\ ]*)$
```

## Installation

The package is on [NuGet](https://www.nuget.org/packages/VerbalExpressions-official) and targets .NET Standard 2.0, so it works on .NET Framework 4.6.1+, .NET Core 2.0+ and .NET 5 and later.

```sh
dotnet add package VerbalExpressions-official
```

Or in the Visual Studio Package Manager Console:

```powershell
Install-Package VerbalExpressions-official
```

Then add the namespace. Note that it is not the same as the package name:

```csharp
using CSharpVerbalExpressions;
```

## Examples

Every example below is also a unit test in [`ReadmeExamplesTests.cs`](https://github.com/VerbalExpressions/CSharpVerbalExpressions/blob/master/VerbalExpressionsUnitTests/ReadmeExamplesTests.cs).

### Testing if we have a valid URL

```csharp
var verbEx = new VerbalExpressions()
    .StartOfLine()
    .Then("http")
    .Maybe("s")
    .Then("://")
    .Maybe("www.")
    .AnythingBut(" ")
    .EndOfLine();

if (verbEx.Test("https://www.google.com"))
{
    Console.WriteLine("We have a correct URL");
}
```

### Replacing strings

```csharp
string result = new VerbalExpressions()
    .Find("bird")
    .Replace("Replace bird with a duck", "duck");

// result: "Replace duck with a duck"
```

### Capturing a value

```csharp
var verbEx = new VerbalExpressions()
    .Find("Order #")
    .BeginCapture("id").Digit().OneOrMore().EndCapture();

string id = verbEx.Capture("Thanks! Order #12345 is on its way.", "id");

// id: "12345"
```

### Partial match or whole input

`Test` returns true if the expression matches anywhere in the input. `TestExact` returns true only if it matches the whole input.

```csharp
var verbEx = new VerbalExpressions().Then("abc").Or("def");

verbEx.Test("defzzz");      // true
verbEx.TestExact("defzzz"); // false
verbEx.TestExact("def");    // true
```

### Reusing expressions

Pass one expression to `Add` to use it inside another. It is wrapped in a non-capturing group, and the original is not changed.

```csharp
var number = new VerbalExpressions().Digit().OneOrMore();

var version = new VerbalExpressions()
    .StartOfLine()
    .Add(number).Then(".").Add(number).Then(".").Add(number)
    .EndOfLine();

version.Test("0.4.0"); // true
version.Test("0.4");   // false
```

### One of several values, ignoring case

```csharp
var verbEx = new VerbalExpressions()
    .StartOfLine()
    .OneOf("jpg", "jpeg", "png")
    .EndOfLine()
    .WithAnyCase();

verbEx.Test("PNG"); // true
verbEx.Test("gif"); // false
```

### Using the underlying Regex

`ToRegex()` returns a normal `System.Text.RegularExpressions.Regex`, so you can use everything .NET offers:

```csharp
Regex regex = new VerbalExpressions().Word().ToRegex();

regex.Matches("three words here").Count; // 3
```

## API reference

All builder methods return the same `VerbalExpressions` instance, so calls can be chained. Text arguments are escaped and matched literally, with one exception: in the character-set methods (`AnyOf`, `AnythingBut`, `SomethingBut`) a `-` between two characters makes a range, so `AnyOf("a-z")` matches any lower-case letter. Put `-` first or last to match it literally. Most methods take an optional `sanitize: false` argument if you want to pass raw regex syntax. The XML documentation ships with the package, so IntelliSense shows every method.

### Building blocks

| Method | Regex | Description |
| --- | --- | --- |
| `StartOfLine()` | `^` | Start of a line |
| `EndOfLine()` | `$` | End of a line |
| `Then(value)` / `Find(value)` | `(value)` | Exactly this text |
| `Maybe(value)` | `(value)?` | This text, optionally |
| `Anything()` | `(.*)` | Any characters, including none |
| `AnythingBut(chars)` | `([^chars]*)` | Any characters except these, including none |
| `Something()` | `(.+)` | At least one character |
| `SomethingBut(chars)` | `([^chars]+)` | At least one character, none of them these |
| `AnyOf(chars)` / `Any(chars)` | `[chars]` | One of these characters |
| `Range('a', 'z', '0', '9')` | `[a-z0-9]` | One character within one or more ranges |
| `OneOf("a", "b", ...)` | `(?:a\|b)` | Exactly one of these values |
| `Or(value)` | `(...)\|(value)` | What came before, or this |
| `Not(value)` | `(?!value)` | Not followed by this (same as `NegativeLookAhead`) |
| `Add(value, sanitize)` | `value` | Append text, or raw regex with `sanitize: false` |
| `Add(expression)` | `(?:...)` | Append another `VerbalExpressions` |
| `Add(CommonRegex.Url)` / `Add(CommonRegex.Email)` | | Append a predefined pattern |

### Special characters and character classes

| Method | Regex | Description |
| --- | --- | --- |
| `LineBreak()` / `Br()` | `(\n\|(\r\n))` | Line break |
| `Tab()` | `\t` | Tab |
| `Word()` | `\w+` | One or more word characters |
| `WordChar()` / `NonWordChar()` | `\w` / `\W` | A single word / non-word character |
| `Digit()` / `NonDigit()` | `\d` / `\D` | A single digit / non-digit |
| `Whitespace()` / `NonWhitespace()` | `\s` / `\S` | A single whitespace / non-whitespace character (also `Space()` / `NonSpace()`) |
| `Letter()` | `[a-zA-Z]` | A single letter |
| `UpperCaseLetter()` / `LowerCaseLetter()` | `[A-Z]` / `[a-z]` | A single upper / lower case letter |
| `WordBoundary()` | `\b` | Word boundary |

### Repetition

| Method | Regex | Description |
| --- | --- | --- |
| `OneOrMore()` | `+` | Previous item one or more times |
| `ZeroOrMore()` | `*` | Previous item zero or more times |
| `AtLeast(n)` | `{n,}` | Previous item at least n times |
| `RepeatPrevious(n)` / `Count(n)` | `{n}` | Previous item exactly n times |
| `RepeatPrevious(n, m)` / `Count(n, m)` | `{n,m}` | Previous item n to m times |
| `Multiple(value)` | `(value)+` | This text one or more times |
| `Multiple(value, min)` | `(value){min,}` | This text at least min times |
| `Multiple(value, min, max)` | `(value){min,max}` | This text min to max times |

### Groups and lookaround

| Method | Regex | Description |
| --- | --- | --- |
| `BeginCapture()` ... `EndCapture()` | `(...)` | Numbered capture group |
| `BeginCapture(name)` ... `EndCapture()` | `(?<name>...)` | Named capture group |
| `BeginGroup()` ... `EndGroup()` | `(?:...)` | Non-capturing group |
| `LookAhead(value)` / `NegativeLookAhead(value)` | `(?=value)` / `(?!value)` | Followed / not followed by this |
| `LookBehind(value)` / `NegativeLookBehind(value)` | `(?<=value)` / `(?<!value)` | Preceded / not preceded by this |

### Options

The expression uses `RegexOptions.Multiline` by default, so `^` and `$` match at every line.

| Method | Description |
| --- | --- |
| `WithAnyCase(bool enable = true)` | Ignore case |
| `SearchOneLine(bool enable = true)` | `^` and `$` match only at the start and end of the whole input (also `UseOneLineSearchOption`) |
| `AddModifier(char)` / `RemoveModifier(char)` | Turn an option on or off: `i` ignore case, `m` multiline, `s` single line, `x` ignore pattern whitespace |
| `WithOptions(RegexOptions)` | Replace all options |

### Using the expression

| Method | Returns | Description |
| --- | --- | --- |
| `Test(input)` / `IsMatch(input)` | `bool` | Matches anywhere in the input |
| `TestExact(input)` | `bool` | Matches the whole input |
| `Capture(input, groupName)` | `string` | Value of a named group in the first match, or null |
| `Replace(input, replacement)` | `string` | Input with every match replaced |
| `ToRegex()` | `Regex` | The compiled regular expression |
| `ToString()` | `string` | The regex pattern |
| `Sanitize(value)` | `string` | The text with regex characters escaped |

### Differences from the JavaScript and Java versions

- Method names use PascalCase, as is normal in C#: `startOfLine()` is `StartOfLine()`.
- `Then`, `Maybe`, `Anything`, `Something`, `AnythingBut`, `SomethingBut` and `Multiple` create **capturing** groups `(...)`. The JavaScript and Java versions create non-capturing groups `(?:...)`. This is kept for backward compatibility. Newer methods such as `OneOf`, `BeginGroup` and `Add(expression)` do not capture.
- JavaScript's `stopAtFirst()` has no C# equivalent, because .NET has no global flag. Use `ToRegex().Match(...)` for the first match and `ToRegex().Matches(...)` for all matches.
- Java's `getText(...)` is not included. Use `Capture(...)` or `ToRegex().Matches(...)`.

## Building and testing

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0). The library itself targets .NET Standard 2.0, and the test project targets .NET 10.

```sh
git clone https://github.com/VerbalExpressions/CSharpVerbalExpressions.git
cd CSharpVerbalExpressions
dotnet build
dotnet test
```

The library is in `VerbalExpressions/` and the NUnit tests are in `VerbalExpressionsUnitTests/`.

## Releasing

1. Update `<Version>` and `<PackageReleaseNotes>` in `VerbalExpressions/VerbalExpressions.csproj`.
2. Create a GitHub release with a matching tag, for example `v0.4.0`.
3. The [Publish NuGet](https://github.com/VerbalExpressions/CSharpVerbalExpressions/blob/master/.github/workflows/publish-nuget.yml) workflow runs the tests, packs the library and pushes it to NuGet.org.

## Contributing

Pull requests are warmly welcome!

1. Fork the repository and create a branch.
2. Add or update tests for your change. If you change an example in this README, also update `ReadmeExamplesTests.cs`.
3. Run `dotnet test` and open a pull request. CI runs the tests on every pull request.

Please keep new methods in line with the [JavaScript](https://github.com/VerbalExpressions/JSVerbalExpressions) and [Java](https://github.com/VerbalExpressions/JavaVerbalExpressions) implementations where possible, using C# naming.

## Other implementations

You can see an up-to-date list of all ports on [VerbalExpressions.github.io](https://VerbalExpressions.github.io).

- [JavaScript](https://github.com/VerbalExpressions/JSVerbalExpressions)
- [Java](https://github.com/VerbalExpressions/JavaVerbalExpressions)
- [Python](https://github.com/VerbalExpressions/PythonVerbalExpressions)
- [PHP](https://github.com/VerbalExpressions/PHPVerbalExpressions)
- [Ruby](https://github.com/ryan-endacott/verbal_expressions)
- [Groovy](https://github.com/VerbalExpressions/GroovyVerbalExpressions)
- [Haskell](https://github.com/VerbalExpressions/HaskellVerbalExpressions)
- [C++](https://github.com/VerbalExpressions/CppVerbalExpressions)
- [Objective-C](https://github.com/VerbalExpressions/ObjectiveCVerbalExpressions)
- [Swift](https://github.com/VerbalExpressions/SwiftVerbalExpressions)

## License

[MIT](https://github.com/VerbalExpressions/CSharpVerbalExpressions/blob/master/LICENSE)
