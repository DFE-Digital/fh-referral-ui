import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on the LocalOfferResults page", () => {
    cy.visit(``);
});

When("the user click on the Apply filters option", () => {
    cy.get('[data-testid="button-apply-filters"]').click();
});

Then("the page directed to itself", () => {
    cy.location('pathname').should('match', new RegExp("/ProfessionalReferral/LocalOfferResults"));
});