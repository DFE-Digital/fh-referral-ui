Feature: Remove Filter Tests

Scenario: Search results page should havelanguage filter
	Given a user as arrived on search page 
	When user searches using postcode 'BS2 0SP' and filters using language 'English'
	Then the user should see the language filter 'English'
