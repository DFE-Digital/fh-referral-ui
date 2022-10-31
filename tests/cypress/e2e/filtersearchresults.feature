Feature: Filter Local Offer Search Results Tests
 Scenario: filter search results using Paid cost option should return valid results
	 Given a user as arrived on search page
	 When user search with a valid postcode 'BS2 0SP'
	 When user filters using Paid option
	 Then the user should see filtered search results