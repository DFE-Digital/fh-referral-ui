Feature: Filter Results by Category Tests

	Scenario: search using valid postcode should return results
		Given a user has arrived on search page 
		When user search with a valid postcode 'BS2 0SP'
		Then the user should see search results

	Scenario: search using Activity
		Given a user has arrived on search page 
		When user search with a valid postcode 'BS2 0SP'
		Then the user should see search results

	Scenario: search using Activities Aid for Children with Tracheostomies should not be in results
		Given a user has arrived on search page 
		When user search with a valid postcode 'BS2 0SP'
		Then the user should see search results
		When a user selects Activities category
		Then Aid for Children with Tracheostomies should not be in results

	Scenario: search using Activities Test Service - Paid - 15 to 20yrs - Afrikaans should be in results
		Given a user has arrived on search page 
		When user search with a valid postcode 'BS2 0SP'
		Then the user should see search results
		When a user selects Activities category
		Then Test Service - Paid - 15 to 20yrs - Afrikaans should be in results