import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on search page", () => {
    cy.visit(`ProfessionalReferral/Search`);
});

When("user search with a valid postcode {string}", (postcode) => {
    cy.get('[data-testid="postcode-value"]').type(postcode);
    cy.get('[data-testid="button-search"]').click();
});

When("a user selects {string} category", (category) => {
    cy.get('[data-testid="' + category +'"]').click();
    cy.get('[data-testid="button-apply-filters"]').click();
});

Then("{string} should not be in results", (service) => {
    cy.get('[data-testid="' + service +'"]').should("not.exist");
});

Then("{string} should be in results", (service) => {
    cy.get('[data-testid="' + service + '"]').should("exist");
});
