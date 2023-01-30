Feature: Filter Results by Category Tests

	Scenario: search using valid postcode should return results
		Given a user has arrived on search page 
		When user search with a valid postcode 'BS2 0SP'
		Then the user should see search results
		Then the user should see 'Services, groups and activities in this area' title
		Then the user should see 'Showing # search results for' message
		Then the user should see 'BS2 0SP' poscode
