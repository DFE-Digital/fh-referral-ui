Feature:Accessibility Statement Page
Scenario: accessiblity page heading be Accessibility Statement
    Given a user has arrived on accessibility statement 
    Then the user should see the accessibility heading

Scenario: accessiblity page should have link to supportemail
    Given a user has arrived on accessibility statement 
    Then the user clicks on mailto link should popup with connect-family-support.service@education.gov.uk

Scenario: accessiblity page should have home button
    Given a user has arrived on accessibility statement 
    Then the user clicks on home button should redirect to home page