import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on search page", () => {
    cy.visit(`ProfessionalReferral/Search`);
});

When("user search with a valid postcode {string}", (postcode) => {
    cy.get('[data-testid="postcode-value"]').type(postcode);
    cy.get('[data-testid="button-search"]').click();
});

Then("the user should see search results", () => {
    cy.get('[data-testid="aidforchildrenwithtracheostomies"]').should("exist");
});

When("use applies Free & In Person filters", () => {
    cy.get('[data-testid="checkbox-free"]').click();
    cy.get('[data-testid="checkbox-inperson"]').click();
    cy.get('[data-testid="button-apply-filters"]').click();
});

Then("the filter cloud should contain these filters", () => {
    cy.get('[data-testid="free-remove"]').should("exist");
    cy.get('[data-testid="1-remove-delivery"]').should("exist");
    cy.get('[data-testid="testservice-free-10to15yrs"]').should("exist");
    cy.get('[data-testid="aidforchildrenwithtracheostomies"]').should("not.exist");
    cy.get('[data-testid="testservice-paid-15to20yrs-afrikaans"]').should("not.exist");
});

When("user removes In Person", () => {
    cy.get('[data-testid="1-remove-delivery"]').click();
});

Then("only one filter should exist", () => {
    cy.get('[data-testid="free-remove"]').should("exist");
    cy.get('[data-testid="1-remove-delivery"]').should("not.exist");
    cy.get('[data-testid="testservice-free-10to15yrs"]').should("exist");
    cy.get('[data-testid="aidforchildrenwithtracheostomies"]').should("exist");
    cy.get('[data-testid="testservice-paid-15to20yrs-afrikaans"]').should("not.exist");
});

When("user clicks on clear all filters", () => {
    cy.get('[data-testid="clearfilters"]').click();
});

Then("all filters should be cleared", () => {
    cy.get('[data-testid="free-remove"]').should("not.exist");
    cy.get('[data-testid="1-remove-delivery"]').should("not.exist");
    cy.get('[data-testid="testservice-free-10to15yrs"]').should("exist");
    cy.get('[data-testid="aidforchildrenwithtracheostomies"]').should("exist");
    cy.get('[data-testid="testservice-paid-15to20yrs-afrikaans"]').should("exist");
});
