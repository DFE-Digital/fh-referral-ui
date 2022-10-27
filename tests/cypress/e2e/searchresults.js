import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user as arrived on search page", () => {
    cy.visit(`ProfessionalReferral/Search`);
});

When("user search with a valid postcode {string}", (postcode) => {
    cy.get('[data-testid="radio-postcode"]').click();
    cy.get('[data-testid="postcode-value"]').type(postcode);
    cy.get('[data-testid="button-search"]').click();
});

Then("the user should see search results", () => {
    cy.get('[data-testid="aidforchildrenwithtracheostomies"]').should("exist");
});


Then("the user should see no results", () => {
    cy.get('[data-testid="aidforchildrenwithtracheostomies"]').should('not.exist');
});