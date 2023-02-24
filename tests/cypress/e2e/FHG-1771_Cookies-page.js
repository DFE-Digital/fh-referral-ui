import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on cookies page", () => {
    cy.visit(`ProfessionalReferral/Cookies`);
});

Then("none of the cookies options are selected", () => {
    cy.get('[data-testid="cookies-accepted-option"]').should('not.be.checked');
    cy.get('[data-testid="cookies-rejected-option"]').should('not.be.checked');
});

Then("user accepts cookies", () => {
    cy.get('[data-testid="cookies-banner-accept-button"]').click();
});

Then("option is set to yes and stays selected after page refresh", () => {
    cy.get('[data-testid="cookies-accepted-option"]').should('be.checked');
    cy.reload();
    cy.get('[data-testid="cookies-accepted-option"]').should('be.checked');
});

Then("user rejects cookies", () => {
    cy.get('[data-testid="cookies-banner-reject-button"]').click();
});

Then("option is set to no and stays selected after page refresh", () => {
    cy.get('[data-testid="cookies-rejected-option"]').should('be.checked');
    cy.reload();
    cy.get('[data-testid="cookies-rejected-option"]').should('be.checked');
});

Then("selects Yes option and saves", () => {
    cy.get('[data-testid="cookies-accepted-option"]').click();
    cy.get('[data-testid="save-cookies"]').click();
});


Then("the cookie is set, success message is shown and when page is refreshed Yes option stays selected", () => {
    cy.getCookie('service_directory_cookies_policy').should('have.property', 'value', 'accept');
    cy.get('[data-testid="cookies-saved-notification"]').should('not.have.css', 'display', 'none');
    cy.reload();
    cy.get('[data-testid="cookies-accepted-option"]').should('be.checked');
    cy.get('[data-testid="cookies-saved-notification"]').should('have.css', 'display', 'none');
});

Then("selects No option and saves", () => {
    cy.get('[data-testid="cookies-rejected-option"]').click();
    cy.get('[data-testid="save-cookies"]').click();
});


Then("the cookie is set, success message is shown and when page is refreshed No option stays selected", () => {
    cy.getCookie('service_directory_cookies_policy').should('have.property', 'value', 'reject');
    cy.get('[data-testid="cookies-saved-notification"]').should('not.have.css', 'display', 'none');
    cy.reload();
    cy.get('[data-testid="cookies-rejected-option"]').should('be.checked');
    cy.get('[data-testid="cookies-saved-notification"]').should('have.css', 'display', 'none');
    
});