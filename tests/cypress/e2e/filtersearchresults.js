import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user as arrived on search page", () => {
    cy.visit(`ProfessionalReferral/Search`);
});

When("user search with a valid postcode {string}", (postcode) => {
    cy.get('[data-testid="radio-postcode"]').click();
    cy.get('[data-testid="postcode-value"]').type(postcode);
    cy.get('[data-testid="button-search"]').click();
});

When("user filters using Paid option", () => {
    cy.get('[data-testid="checkbox-paid"]').click();
    cy.get('[data-testid="button-apply-filters"]').click();
});

Then("the user should see filtered search results", () => {
    cy.get('[data-testid="testservice-paid"]').should("exist");
});