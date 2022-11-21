import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on the start now page", () => {
    cy.visit(``);
});

Then("the heading should say {string}", (heading) => {
    cy.get("h1").should("contain.text", heading);
});

Then("the page has a list class", () => {
    cy.get('[data-testid="list-id"]').should("exist");
});

When("the user click on the start now option", () => {
    cy.get('[data-testid="button-start"]').click();
});

Then("the page directed to sign into your account page", () => {
    cy.location('pathname').should('match', new RegExp("/ProfessionalReferral/SignIn"));
});

