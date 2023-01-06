Feature: Search Results Details Tests

 Scenario:Search Details heading is 'Aid for Children with Tracheostomies'
    Given a user has arrived on search results page by a search with a valid postcode 'E1 2AD'
     When user click on the  results
     Then search results page should show heading 'Test Service 1' 

 Scenario:Search Details page has cost value 
    Given a user has arrived on search results page by a search with a valid postcode 'E1 2AD'
     When user click on the  results
     Then search results page should show cost has 'Free'


 Scenario:Search Details page has back link to search results page 
    Given a user has arrived on search results page by a search with a valid postcode 'E1 2AD'
     When user click on the  results
     Then user clicks on the back button he should be redirected to search results page
