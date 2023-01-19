Feature: Filter Cloud

	Scenario: search using Free & In Person
		Given a user has arrived on search page 
		When user search with a valid postcode 'BS2 0SP'
		Then the user should see search results
		When use applies Free & In Person filters
		Then the filter cloud should contain these filters

	Scenario: remove In Person filter
		Given a user has arrived on search page 
		When user search with a valid postcode 'BS2 0SP'
		When use applies Free & In Person filters
		When user removes In Person
		Then only one filter should exist

	Scenario: clear all filters
		Given a user has arrived on search page 
		When user search with a valid postcode 'BS2 0SP'
		When use applies Free & In Person filters
		When user clicks on clear all filters
		Then all filters should be cleared
