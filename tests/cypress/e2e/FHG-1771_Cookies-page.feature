Feature: Cookies page

	Scenario: User has no cookies set
		Given a user has arrived on cookies page
		Then none of the cookies options are selected

	Scenario: User accepts cookies and Yes option is selected immediatealy and stays selected after page refresh
		Given a user has arrived on cookies page
		Then user accepts cookies
		Then option is set to yes and stays selected after page refresh

	Scenario: User rejects cookies and No option is selected immediatealy and stays selected after page refresh
		Given a user has arrived on cookies page
		Then user rejects cookies
		Then option is set to no and stays selected after page refresh

	Scenario: User accepts via Yes option
		Given a user has arrived on cookies page
		Then none of the cookies options are selected
		Then selects Yes option and saves
		Then the cookie is set and when page is refreshed Yes option stays selected

	Scenario: User accepts via No option
		Given a user has arrived on cookies page
		Then none of the cookies options are selected
		Then selects No option and saves
		Then the cookie is set and when page is refreshed No option stays selected