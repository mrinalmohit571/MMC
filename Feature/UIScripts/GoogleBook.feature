Feature: Google Book
	
@UIScript
Scenario: Search Google Book
	Given User navigate to Google_UI Search Page
	And Enter Book in the search box
	When Click the search button
	And Go to the Book section
	Then Click on the Book result
	And Verify it open a correct link