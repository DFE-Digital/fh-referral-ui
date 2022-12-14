import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on the start page", () => {
    cy.visit(``);
});

Then("the body element should have dfe-template__body class", () => {
    cy.get('[data-testid="pagebody"]').should('have.class', 'dfe-template__body');
});

Then("the header title should say {string}", (title) => {
    cy.get('[data-testid="headertitle"]').should("contain.text", title);
});

Then("the header sub-title should say {string}", (subtitle) => {
    cy.get('[data-testid="headersubtitle"]').should("contain.text", subtitle);
});

When("the user clicks on the logo title link", () => {
    cy.get('[data-testid="headertitle"]').click();
});

Then("the page directed to DFE website", () => {
    cy.url().should('eq', 'https://www.gov.uk/government/organisations/department-for-education');
});

When("the user clicks on the logo sub-title link", () => {
    cy.get('[data-testid="headersubtitle"]').click();
});

Then("the page directed to start now page", () => {
    cy.location('pathname').should('match', new RegExp("/"));
});