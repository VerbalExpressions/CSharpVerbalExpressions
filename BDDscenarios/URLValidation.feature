Feature: URLValidation

Scenario: A valid URL is evaluated valid
	Given I have a valid URL "http://google.com"
	When I test it with the appropriate methods
	Then I get a true assertion
