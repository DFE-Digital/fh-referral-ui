Feature: Search Results Details Should contain Phone, Email, Address and website data

 Scenario:Search Details contains Phone field
    Given a user has arrived on search results page by a search with a valid postcode 'E1 2AD'
     When user user arrives on search details page
     Then search results page should contain Phone

 Scenario:Search Details contains Email field
    Given a user has arrived on search results page by a search with a valid postcode 'E1 2AD'
     When user user arrives on search details page
     Then search results page should contain Email

 Scenario:Search Details contains Address field
    Given a user has arrived on search results page by a search with a valid postcode 'E1 2AD'
     When user user arrives on search details page
     Then search results page should contain Address

 Scenario:Search Details contains Website field
    Given a user has arrived on search results page by a search with a valid postcode 'E1 2AD'
     When user user arrives on search details page
     Then search results page should contain Website