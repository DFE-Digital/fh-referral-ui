Feature: Filter Local Offer Search Results By family location choice

	Scenario: filter should exist
	Given a user has arrived on search page
	When user searches by a valid postcode 'BS2 0SP'
	Then the user should see family can choose location filter

	Scenario: show local services that offer option for families to choose the location
	Given a user has arrived on search results page using a valid postcode 'BS2 0SP'
	When user filters using option for families to choose the location as checked
	Then user is presented with local services that offer option for families to choose the location