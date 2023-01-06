import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on search page", () => {
    cy.visit(`ProfessionalReferral/Search`);
});

When("user search with a valid postcode {string}", (postcode) => {
    cy.get('[data-testid="postcode-value"]').type(postcode);
    cy.get('[data-testid="button-search"]').click();
});

Then("the user should see search results", () => {
    cy.get('[data-testid="testservice1"]').should("exist");
});

When("use applies German & Online filters", () => {
    cy.get('[data-testid="select-language"]').select("German");
    cy.get('[data-testid="checkbox-online"]').click();
    cy.get('[data-testid="button-apply-filters"]').click();
});

Then("the filter cloud should contain these filters", () => {
    cy.get('[data-testid="2-remove-delivery"]').should("exist");
    cy.get('[data-testid="German-remove"]').should("exist");
    cy.get('[data-testid="testservice6"]').should("exist");
    cy.get('[data-testid="testservice2"]').should("not.exist");
    cy.get('[data-testid="testservice11"]').should("not.exist");
    cy.get('[data-testid="testservice13"]').should("not.exist");
});

When("user removes German", () => {
    cy.get('[data-testid="German-remove"]').click();
});

Then("only one filter should exist", () => {
    cy.get('[data-testid="2-remove-delivery"]').should("exist");
    cy.get('[data-testid="German-remove"]').should("not.exist");
    cy.get('[data-testid="testservice1"]').should("not.exist");
    cy.get('[data-testid="testservice6"]').should("exist");
    cy.get('[data-testid="testservice2"]').should("exist");
    cy.get('[data-testid="testservice11"]').should("exist");
    cy.get('[data-testid="testservice13"]').should("exist");
});

When("user clicks on clear all filters", () => {
    cy.get('[data-testid="clearfilters"]').click();
});

Then("all filters should be cleared", () => {
    cy.get('[data-testid="2-remove-delivery"]').should("not.exist");
    cy.get('[data-testid="German-remove"]').should("not.exist");
});
