Feature: Search Results Details Tests

 Scenario:Search Details heading is 'Aid for Children with Tracheostomies'
    Given a user has arrived on search results page by a search with a valid postcode 'BS2 0SP'
     When user click on the  results
     Then search results page should show heading 'Aid for Children with Tracheostomies' 

 Scenario:Search Details page has cost value 
    Given a user has arrived on search results page by a search with a valid postcode 'BS2 0SP'
     When user click on the  results
     Then search results page should show cost has 'Free'


 Scenario:Search Details page has back link to search results page 
    Given a user has arrived on search results page by a search with a valid postcode 'BS2 0SP'
     When user click on the  results
     Then user clicks on the back button he should be redirected to search results page

 Scenario:Search Details page has back link to correct search results page when there are more than one pages
    Given a user has arrived on search results page by a search with a valid postcode 'BS2 0SP'
     When user navigates to 'page_2' and click on the service 'testservice15'
     Then user clicks on the back button he should be redirected to search results page two and be able to see the service 'testservice15'
