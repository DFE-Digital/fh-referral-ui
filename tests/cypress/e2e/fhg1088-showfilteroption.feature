Feature: Search Results Tests

	Scenario: On mobile we should see the Show filters button which displays filter on overlay
		Given a user has arrived on search page
		When user searches with a valid postcode 'E1 2AD'
		Then the user should see search results
		When user is in desktop mode
		Then the user should not see show filters or return to results buttons
		When user is in mobile mode
		Then the user should see show filters button
		When user clicks on show filters button
		Then filters overlay should be visible
		When user clicks on return to results button
		Then filters overlay should close
