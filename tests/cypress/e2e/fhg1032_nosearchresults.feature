Feature: Search Results Tests

	Scenario: search using valid postcode with no results
	Given a user as arrived on search page 
	When user search with a valid postcode 'WF8 1LD'
	Then the user should see no results