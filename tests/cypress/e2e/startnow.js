import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on the start now page", () => {
    cy.visit(``);
});

Then("the heading should say {string}", (heading) => {
    cy.title().should("contain.text", heading);
});

Then("the page's title is {title}", (title) => {
    cy.get("h1").should("equal", title);
});

Then("the page has a list class", () => {
    cy.get('[data-testid="list-id"]').should("exist");
});

When("the user click on the start now option", () => {
    cy.get('[data-testid="button-start"]').click();
});

Then("the page directed to search page", () => {
    cy.location('pathname').should('match', new RegExp("/ProfessionalReferral/Search"));
});

When("the user click on the child abuse link", () => {
    cy.get('[data-testid="child-abuse-link"]').click();
});

Then("the page directed to child abuse url", () => {
    cy.url().should('eq', 'https://www.gov.uk/report-child-abuse')
});