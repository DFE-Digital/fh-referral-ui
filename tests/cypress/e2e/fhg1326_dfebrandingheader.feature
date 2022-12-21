Feature: DFE Header Tests

   Scenario: Body element has the DFE class
    Given a user has arrived on the start page
    Then the body element should have dfe-template__body class

   Scenario: Header title is 'Department for Education'
    Given a user has arrived on the start page
    Then the header title should say 'Department for Education'

   Scenario: Header sub-title is 'Connect families to support'
    Given a user has arrived on the start page
    Then the header sub-title should say 'Connect families to support'

   Scenario: DFE logo title should link to DFE website
     Given a user has arrived on the start page
     When the user clicks on the logo title link
     Then the page directed to DFE website

   Scenario: DFE logo sub-title should link to start now page
     Given a user has arrived on the start page
     When the user clicks on the logo sub-title link
     Then the page directed to start now page