Feature: Filter Local Offer Search Results By Age Tests

	Scenario: children and young people filter should exist
	Given a user has arrived on search page
	When user searches by a valid postcode 'BS2 0SP'
	Then the user should see children and young people filter

	Scenario: show local services that offer children and young people services for the selected age
	Given a user has arrived on search results page using a valid postcode 'BS2 0SP'
	When user filters using age = 14
	Then user is presented with local services that offer children and young people services for the selected age