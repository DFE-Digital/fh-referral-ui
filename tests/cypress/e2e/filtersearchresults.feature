Feature: Filter Local Offer Search Results Tests
 Scenario: filtering search results using cost options should return valid results
	 Given a user as arrived on search page
	 When user search with a valid postcode 'BS2 0SP'
	 When user filters using Paid option
	 Then the user should see a paid service 'testservice-paid-0to13yrs'
	 When user filters using Free option
	 Then the user should see a free service 'testservice-free-10to15yrs'
	 When user filters using Paid and Free options
	 Then the user should see a free service 'testservice-free-10to15yrs' and a paid service 'testservice-free-10to15yrs'