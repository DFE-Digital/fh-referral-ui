Feature: Filter Local Offer Search Results By Delivery methods Tests

	Scenario: previous servcies are displayed after clearing filters
	Given a user has arrived on search page
	When user searches using postcode 'BS2 0SP'
	Then user can see searvice 'aidforchildrenwithtracheostomies'
	When user filters using language 'Xhosa'
	Then the user can not see the service 'aidforchildrenwithtracheostomies'
	When user hits clear filters
	Then user can again see the service 'aidforchildrenwithtracheostomies'