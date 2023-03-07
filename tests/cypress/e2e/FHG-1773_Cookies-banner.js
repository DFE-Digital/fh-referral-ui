import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on home page", () => {
    cy.visit(`/`);
});

Then("the cookies banner should display", () => {
    cy.get('[data-testid="cookies-banner"]').should("exist");
});

When("user accepts cookies", () => {
    cy.get('[data-testid="cookies-banner-accept-button"]').click();
});

Then("the Accepted confirmation should show and cookie is set", () => {
    cy.get('[data-testid="cookies-banner-message"]').should('have.css', 'display', 'none');
    cy.get('[data-testid="cookies-banner-confirmation"]').should('not.have.css', 'display', 'none');
    cy.get('[data-testid="cookies-banner-reject-message"]').should('have.css', 'display', 'none');
    cy.get('[data-testid="cookies-banner-accept-message"]').should('not.have.css', 'display', 'none');
    cy.getCookie('service_directory_cookies_policy').should('have.property', 'value', 'accept');
});

When("user rejects cookies", () => {
    cy.get('[data-testid="cookies-banner-reject-button"]').click();
});

Then("the Rejected confirmation should show and cookie is set", () => {
    cy.get('[data-testid="cookies-banner-message"]').should('have.css', 'display', 'none');
    cy.get('[data-testid="cookies-banner-confirmation"]').should('not.have.css', 'display', 'none');
    cy.get('[data-testid="cookies-banner-reject-message"]').should('not.have.css', 'display', 'none');
    cy.get('[data-testid="cookies-banner-accept-message"]').should('have.css', 'display', 'none');
    cy.getCookie('service_directory_cookies_policy').should('have.property', 'value', 'reject');
});

When("user clicks on Hide this message", () => {
    cy.get('[data-testid="cookies-banner-close"]').click();
});

Then("the confirmation message should close", () => {
    cy.get('[data-testid="cookies-banner"]').should('have.css', 'display', 'none');
});
