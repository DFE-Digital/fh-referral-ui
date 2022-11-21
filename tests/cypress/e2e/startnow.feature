Feature: Start Now Page Tests
 Scenario: start now heading is 'Connect families to voluntary and community services'
    Given a user has arrived on the start now page
    Then the heading should say 'Connect families to voluntary and community services'

   Scenario: start now has a list class
     Given a user has arrived on the start now page
    Then the page has a list class

   Scenario: start now page has a  start now button
     Given a user has arrived on the start now page
     When the user click on the start now option
     Then the page directed to sign into your account page