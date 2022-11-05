Feature: Filter Local Offer Search Results By Delivery methods Tests

	Scenario: delivery methods filters should exist
	Given a user has arrived on search page
	When user searches by a valid postcode 'BS2 0SP'
	Then the user should see delivery methods filters in-person, online and telephone

	Scenario: show local services that offer online services
	When user searches using postcode 'BS2 0SP' and filters using delivery method 'Online'
	Then the user should see the service 'aidforchildrenwithtracheostomies'

	Scenario: show local services that offer in-person services
	When user searches using postcode 'BS2 0SP' and filters using delivery method 'In person'
	Then the user should see the service 'testservice-free-10to15yrs'

	Scenario: show local services that offer telephone services
	When user searches using postcode 'BS2 0SP' and filters using delivery method 'Telephone'
	Then the user should see the service 'testservice-paid-0to13yrs'
	
	Scenario: show local services that offer both in-person and online services
	When user searches using postcode 'BS2 0SP' and filters using delivery methods 'In person' and 'Online'
	Then the user should see the services 'aidforchildrenwithtracheostomies' and 'testservice-free-10to15yrs'

	Scenario: show local services that offer all three delivery methods
	When user searches using postcode 'BS2 0SP' and filters using all delivery methods
	Then the user should see the services 'aidforchildrenwithtracheostomies' , 'testservice-free-10to15yrs' and 'testservice-paid-0to13yrs'

	

	

