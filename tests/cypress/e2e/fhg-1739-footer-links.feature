Feature: Page Footer Links 
Scenario: accessiblity link to page
    Given a user has started the referral ui journey
    Then the user should see and able to click on  footer link 'accessibility-link' links to '/ProfessionalReferral/Accessibility'

Scenario: contact us link to page
    Given a user has started the referral ui journey
    Then the user should see and able to click on  footer link 'contact-us-link' links to '/ProfessionalReferral/Contactus'

Scenario: cookies link to page
    Given a user has started the referral ui journey
    Then the user should see and able to click on  footer link 'cookies-link' links to '/ProfessionalReferral/Cookies'

Scenario: termsandcondition link to page
    Given a user has started the referral ui journey
    Then the user should see and able to click on  footer link 'terms-condition-link' links to '/ProfessionalReferral/Termsandcondition'

Scenario: feedback link to feedback form
    Given a user has started the referral ui journey
    Then the user should see and able to click on feedback form redirect to form