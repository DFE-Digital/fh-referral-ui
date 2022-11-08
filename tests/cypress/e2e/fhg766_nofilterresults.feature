Feature: Filter Local Offer Search Results By Delivery methods Tests

	Scenario: no results message should be displayed when no results returned after filtering
	Given a user has arrived on search page
	When user searches using postcode 'BS2 0SP' and filters using language 'Xhosa'
	Then the user should see the heading 'noservicesfound' and message 'trysearchingagain'