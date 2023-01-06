Feature: Filter Results by Category Tests

	Scenario: search using Activities
		Given a user has arrived on search page 
		When user search with a valid postcode 'E1 2AD'
		When a user selects "activities" category
		Then "testservice1" should be in results
		Then "testservice2" should not be in results

	Scenario: search using Before and after school clubs
		Given a user has arrived on search page 
		When user search with a valid postcode 'E1 2AD'
		When a user selects "beforeandafterschoolclubs" category
		Then "testservice2" should be in results
		Then "testservice1" should not be in results

	Scenario: search using Holiday clubs and schemes
		Given a user has arrived on search page 
		When user search with a valid postcode 'E1 2AD'
		When a user selects "holidayclubsandschemes" category
		Then "testservice1" should not be in results
		Then "testservice3" should be in results

	Scenario: search using Music, arts and dance
		Given a user has arrived on search page 
		When user search with a valid postcode 'E1 2AD'
		When a user selects "music,artsanddance" category
		Then "testservice1" should not be in results
		Then "testservice4" should be in results

	Scenario: search using Parent, baby and toddler groups
		Given a user has arrived on search page 
		When user search with a valid postcode 'E1 2AD'
		When a user selects "parent,babyandtoddlergroups" category
		Then "testservice1" should not be in results
		Then "testservice12" should be in results

	Scenario: search using Pre-school playgroup
		Given a user has arrived on search page 
		When user search with a valid postcode 'E1 2AD'
		When a user selects "pre-schoolplaygroup" category
		Then "testservice1" should not be in results
		Then "testservice5" should be in results

	Scenario: search using Sports and recreation
		Given a user has arrived on search page 
		When user search with a valid postcode 'E1 2AD'
		When a user selects "sportsandrecreation" category
		Then "testservice1" should not be in results
		Then "testservice6" should be in results

	Scenario: search using Bullying and cyber bullying
		Given a user has arrived on search page 
		When user search with a valid postcode 'E1 2AD'
		When a user selects "bullyingandcyberbullying" category
		Then "testservice1" should not be in results
		Then "testservice7" should be in results

	Scenario: search using Debt and welfare advice
		Given a user has arrived on search page 
		When user search with a valid postcode 'E1 2AD'
		When a user selects "debtandwelfareadvice" category
		Then "testservice1" should not be in results
		Then "testservice8" should be in results

	Scenario: search using Domestic abuse
		Given a user has arrived on search page 
		When user search with a valid postcode 'E1 2AD'
		When a user selects "domesticabuse" category
		Then "testservice1" should not be in results
		Then "testservice9" should be in results

	Scenario: search using Intensive targeted family support
		Given a user has arrived on search page 
		When user search with a valid postcode 'E1 2AD'
		When a user selects "intensivetargetedfamilysupport" category
		Then "testservice1" should not be in results
		Then "testservice10" should be in results


