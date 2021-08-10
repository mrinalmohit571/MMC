Feature: Google Map

@UIScript
Scenario: Search Google MAP
	Given User navigate to Google_UI Search Page
	And Enter Map in the search box
	When Click the search button
	And Go to the Map section
	Then verify it open Map in google map
	And User Zoom the map out until the map scale reaches the value Map