Feature: Search Results Tests
 Scenario: search using valid postcode should return a valid results
 Given a user as arrived on search page 
 When user search with a valid postcode 'BS2 0SP'
 Then the user should see search results

 Scenario: search using valid postcode with no results
 Given a user as arrived on search page 
 When user search with a valid postcode 'WF8 1LD'
 Then the user should see no results