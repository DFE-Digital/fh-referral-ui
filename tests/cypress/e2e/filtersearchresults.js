import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user as arrived on search page", () => {
    cy.visit(`ProfessionalReferral/Search`);
});

When("user search with a valid postcode {string}", (postcode) => {
    cy.get('[data-testid="radio-postcode"]').click();
    cy.get('[data-testid="postcode-value"]').type(postcode);
    cy.get('[data-testid="button-search"]').click();
});

//Paid
When("user filters using Paid option", () => {
    cy.get('[data-testid="checkbox-paid"]').click();
    cy.get('[data-testid="button-apply-filters"]').click();
});

Then("the user should see a paid service 'testservice-paid-0to13yrs'", () => {
    cy.get('[data-testid="testservice-paid-0to13yrs"]').should("exist");
});

//Free
When("user filters using Free option", () => {
    cy.get('[data-testid="checkbox-free"]').click();
    cy.get('[data-testid="button-apply-filters"]').click();
});

Then("the user should see a free service 'testservice-free-10to15yrs'", () => {
    cy.get('[data-testid="testservice-free-10to15yrs"]').should("exist");
});

//Paid and free
When("user filters using Paid and Free options", () => {
    cy.get('[data-testid="checkbox-paid"]').click();
    cy.get('[data-testid="checkbox-free"]').click();
    cy.get('[data-testid="button-apply-filters"]').click();
});

Then("the user should see a free service 'testservice-free-10to15yrs' and a paid service 'testservice-free-10to15yrs'", () => {
    cy.get('[data-testid="testservice-free-10to15yrs"]').should("exist");
    cy.get('[data-testid="testservice-paid-0to13yrs"]').should("exist");
});

//Age
Given("the user visits search page again", () => {
    cy.visit(`ProfessionalReferral/Search`);
});

When("user enters a valid postcode {string}", (postcode) => {
    cy.get('[data-testid="radio-postcode"]').click();
    cy.get('[data-testid="postcode-value"]').type(postcode);
    cy.get('[data-testid="button-search"]').click();
});

When("user filters using Age = 14", () => {
    cy.get('[data-testid="checkbox-children"]').click();
    cy.get('[data-testid="select-children"]').select("14");
    cy.get('[data-testid="button-apply-filters"]').click();
});

Then("the user should see filtered search results", () => {
    cy.get('[data-testid="testservice-free-10to15yrs"]').should("exist");
});