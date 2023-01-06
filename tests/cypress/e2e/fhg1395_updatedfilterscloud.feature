Feature: Filter Cloud

	Scenario: search using German & Online
		Given a user has arrived on search page 
		When user search with a valid postcode 'E1 2AD'
		Then the user should see search results
		When use applies German & Online filters
		Then the filter cloud should contain these filters

	Scenario: remove German filter
		Given a user has arrived on search page 
		When user search with a valid postcode 'E1 2AD'
		When use applies German & Online filters
		When user removes German
		Then only one filter should exist

	Scenario: clear all filters
		Given a user has arrived on search page 
		When user search with a valid postcode 'E1 2AD'
		When use applies German & Online filters
		When user clicks on clear all filters
		Then all filters should be cleared
