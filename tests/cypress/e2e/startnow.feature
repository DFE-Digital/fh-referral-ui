Feature: Start Now Page Tests
 Scenario: start now heading is 'Connect families to support'
    Given a user has arrived on the start now page
    Then the heading should say 'Connect families to support'

   Scenario: start now has a list class
     Given a user has arrived on the start now page
    Then the page has a list class

   Scenario: start now page has a  start now button
     Given a user has arrived on the start now page
     When the user click on the start now option
     Then the page directed to search page

   Scenario: start now page has a child abuse link
     Given a user has arrived on the start now page
     When the user click on the child abuse link
     Then the page directed to child abuse url

    Scenario: start page title is 'Connect families to support'
    Given a user has arrived on the start now page
    Then the page's title is 'Connect families to support'