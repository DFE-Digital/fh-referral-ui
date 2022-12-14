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

When("a user selects Activities  & Hearing & Sight categories", () => {
    cy.get('[data-testid="activities,clubsandgroups"]').click();
    cy.get('[data-testid="activities"]').click();
    cy.get('[data-testid="health"]').click();
    cy.get('[data-testid="hearingandsight"]').click();
    cy.get('[data-testid="button-apply-filters"]').click();
});

Then("we should get 3 results", () => {
    cy.get('[data-testid="aidforchildrenwithtracheostomies"]').should("exist");
    cy.get('[data-testid="testservice-free-10to15yrs"]').should("exist");
    cy.get('[data-testid="testservice-paid-15to20yrs-afrikaans"]').should("exist");
});

When("a user selects Activities  & Hearing & Sight categories and Free option", () => {
    cy.get('[data-testid="activities,clubsandgroups"]').click();
    cy.get('[data-testid="activities"]').click();
    cy.get('[data-testid="health"]').click();
    cy.get('[data-testid="hearingandsight"]').click();
    cy.get('[data-testid="checkbox-free"]').click();
    cy.get('[data-testid="button-apply-filters"]').click();
});

Then("we should get 2 results", () => {
    cy.get('[data-testid="aidforchildrenwithtracheostomies"]').should("exist");
    cy.get('[data-testid="testservice-free-10to15yrs"]').should("exist");
    cy.get('[data-testid="testservice-paid-15to20yrs-afrikaans"]').should("not.exist");
});
