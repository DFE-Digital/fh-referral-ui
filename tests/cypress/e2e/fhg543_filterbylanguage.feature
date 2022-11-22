Feature: Filter Local Offer Search Results By Delivery methods Tests

	Scenario: language filter should exist
	Given a user has arrived on search page
	When user searches by a valid postcode 'BS2 0SP'
	Then the user should see language filter

	Scenario: show local services offered in Afrikaans
	When user searches using postcode 'BS2 0SP' and filters using language 'Afrikaans'
	Then the user should see the service 'testservice-paid-15to20yrs-afrikaans'

	Scenario: show local services offered in English
	When user searches using postcode 'BS2 0SP' and filters using language 'English'
	Then the user should see the service 'aidforchildrenwithtracheostomies'