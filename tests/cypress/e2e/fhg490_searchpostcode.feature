Feature:  Search for Services by Postcode Tests

Scenario: search with valid postcode
	Given a user has arrived on search page
	When user search by a postcode 'E1 2AD'
    Then the user should see search results


Scenario: search with invalid postcode
	Given a user has arrived on search page
	When user search by a postcode '12345'
    Then the user should see error message for invalid postcode

Scenario: search with empty postcode
	Given a user has arrived on search page
	When user search by a empty postcode 
    Then the user should see error message for empty postcode

Scenario: clicking back button should navigate to start page
	When user clicks the back button
	Then the application should navigate to start page

