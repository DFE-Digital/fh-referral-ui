Feature: Filter Results by Category Tests

	Scenario: search using valid postcode should return results
		Given a user has arrived on search page 
		When user search with a valid postcode 'BS2 0SP'
		Then the user should see search results

	Scenario: search using Activities & Hearing & Sight
		Given a user has arrived on search page 
		When user search with a valid postcode 'BS2 0SP'
		Then the user should see search results
		When a user selects Activities  & Hearing & Sight categories
		Then we should get 3 results

	Scenario: search using Activities & Hearing & Sight & Free
		Given a user has arrived on search page 
		When user search with a valid postcode 'BS2 0SP'
		Then the user should see search results
		When a user selects Activities  & Hearing & Sight categories and Free option
		Then we should get 2 results