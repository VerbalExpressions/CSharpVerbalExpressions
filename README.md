CSharpVerbalExpressions
=====================

## CSharp Regular Expressions made easy
VerbalExpressions is a CSharp library that helps to construct difficult regular expressions.

## NuGet
```
Install-Package VerbalExpressions-official
```

## How to get started
When first building the solution there will be external libraries that are missing since GitHub doesn't include DLLs. 
The best way to get these libraries into your solution is to use NuGet. However, since the project is now using NuGet 
Package Restore, manually installing the packages may not be necessary. Below lists the libraries that are required
if manual installing is needed.

The libraries that are needed to build are the following:
* NUnit

## Examples

Here's a couple of simple examples to give an idea of how VerbalExpressions works:

### Testing if we have a valid URL

```csharp

		[TestMethod]
		public void TestingIfWeHaveAValidURL()
		{
			// Create an example of how to test for correctly formed URLs
			var verbEx = new VerbalExpressions()
						.StartOfLine()
						.Then( "http" )
						.Maybe( "s" )
						.Then( "://" )
						.Maybe( "www." )
						.AnythingBut( " " )
						.EndOfLine();

			// Create an example URL
			var testMe = "https://www.google.com";

			Assert.IsTrue(verbEx.Test( testMe ), "The URL is incorrect");

			Console.WriteLine("We have a correct URL ");
		}

```
## API documentation

Coming...

## Other implementations  
You can view all implementations on [VerbalExpressions.github.io](http://VerbalExpressions.github.io)
