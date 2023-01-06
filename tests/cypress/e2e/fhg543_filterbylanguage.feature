Feature: Filter Local Offer Search Results By Delivery methods Tests

	Scenario: language filter should exist
	Given a user has arrived on search page
	When user searches by a valid postcode 'E1 2AD'
	Then the user should see language filter

	Scenario: show local services offered in German
	When user searches using postcode 'E1 2AD' and filters using language 'German'
	Then the user should see the service 'testservice5'
	Then the user should see the service 'testservice6'
	Then the user should see the service 'testservice7'
	Then the user should not see the service 'testservice8'
	Then the user should not see the service 'testservice9'
	Then the user should not see the service 'testservice10'

	Scenario: show local services offered in French
	When user searches using postcode 'E1 2AD' and filters using language 'French'
	Then the user should not see the service 'testservice5'
	Then the user should not see the service 'testservice6'
	Then the user should not see the service 'testservice7'
	Then the user should see the service 'testservice8'
	Then the user should see the service 'testservice9'
	Then the user should see the service 'testservice10'