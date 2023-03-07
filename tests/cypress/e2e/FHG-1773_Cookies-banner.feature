Feature: Cookies banner

	Scenario: Accept cookies
		Given a user has arrived on home page
		Then the cookies banner should display
		When user accepts cookies
		Then the Accepted confirmation should show and cookie is set

	Scenario: Reject cookies
		Given a user has arrived on home page
		Then the cookies banner should display
		When user rejects cookies
		Then the Rejected confirmation should show and cookie is set

	Scenario: Close confirmation message
		Given a user has arrived on home page
		Then the cookies banner should display
		When user rejects cookies
		Then the Rejected confirmation should show and cookie is set
		When user clicks on Hide this message
		Then the confirmation message should close