Feature: Zippopotam
	Simple calculator for adding two numbers

@APIScript
Scenario Outline: Retrieve the City names 
	Given User define Application as zippopotam_API
	And User define zip code <ZipCode>
	When user request for response
	Then user verify response data OK
	And Verify country name must be returned correctly <ZipCode>
	And verify number of entry should be one
	And Verify response time needs to be under expected time limit <ZipCode>
	Examples: 
	| ZipCode |
	| zip1    |
	| zip2    |
	| zip3    |
