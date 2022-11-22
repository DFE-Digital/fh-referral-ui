Feature: Search Results Tests

	Scenario: search using valid postcode with no results
	Given a user as arrived on search page 
	When user search with a valid postcode 'WF8 1LD'
	Then the user should see no results
	When user clicks the try using different postcode hyperlink
	Then the application should navigate to Search