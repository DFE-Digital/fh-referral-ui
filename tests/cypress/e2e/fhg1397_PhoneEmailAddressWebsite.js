import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";


Given("a user has arrived on search results page by a search with a valid postcode {string}", (postcode) => {
    cy.visit(`ProfessionalReferral/Search`);
    cy.get('[data-testid="postcode-value"]').type(postcode);
    cy.get('[data-testid="button-search"]').click();
});

When("user user arrives on search details page", () => {
    cy.get('[data-testid="testservice1"]').click();
});

Then("search results page should contain Phone", () => {
    cy.get('[data-testid="detailsphone"]').should("exist");
});

Then("search results page should contain Email", () => {
    cy.get('[data-testid="detailsphone"]').should("exist");
});

Then("search results page should contain Address", () => {
    cy.get('[data-testid="detailsphone"]').should("exist");
});

Then("search results page should contain Website", () => {
    cy.get('[data-testid="detailsphone"]').should("exist");
});