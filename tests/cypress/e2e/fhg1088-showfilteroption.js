import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on search page", () => {
    cy.visit(`ProfessionalReferral/Search`);
});

When("user searches with a valid postcode {string}", (postcode) => {
    cy.get('[data-testid="postcode-value"]').type(postcode);
    cy.get('[data-testid="button-search"]').click();
});

Then("the user should see search results", () => {
    cy.get('[data-testid="testservice1"]').should("exist");
});

When("user is in desktop mode", () => {
    cy.viewport(1440, 900);
});

Then("the user should not see show filters or return to results buttons", () => {
    cy.get('[data-testid="show-filters-button"]').should('not.be.visible');
    cy.get('[data-testid="return-to-results-link"]').should('not.be.visible');
});

When("user is in mobile mode", () => {
    cy.viewport(375, 812);
});

Then("the user should see show filters button", () => {
    cy.get('[data-testid="show-filters-button"]').should('be.visible');
});

When("user clicks on show filters button", () => {
    cy.get('[data-testid="show-filters-button"]').click();
});

Then("filters overlay should be visible", () => {
    cy.get('[data-testid="filtersoverlay"]').should('be.visible');
});

When("user clicks on return to results button", () => {
    cy.get('[data-testid="return-to-results-link"]').click();
});

Then("filters overlay should close", () => {
    cy.get('[data-testid="filtersoverlay"]').should('not.be.visible');
});
