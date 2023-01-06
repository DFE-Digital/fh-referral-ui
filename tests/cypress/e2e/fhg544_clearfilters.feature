Feature: Filter Local Offer Search Results By Delivery methods Tests

	Scenario: previous servcies are displayed after clearing filters
	Given a user has arrived on search page
	When user searches using postcode 'E1 2AD'
	Then user can see searvice 'testservice1'
	When user filters using language 'Xhosa'
	Then the user can not see the service 'testservice1'
	When user hits clear filters
	Then user can again see the service 'testservice1'