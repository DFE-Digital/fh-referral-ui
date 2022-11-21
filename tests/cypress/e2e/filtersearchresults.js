import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user as arrived on search page", () => {
    cy.visit(`ProfessionalReferral/Search`);
});

When("user search with a valid postcode {string}", (postcode) => {
        cy.searchbypostcode(postcode);
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
    cy.visit(`ProfessionalReferral/Search`);
    cy.searchbypostcode('BS2 0SP');
    cy.get('[data-testid="checkbox-free"]').click();
    cy.get('[data-testid="button-apply-filters"]').click();
});

Then("the user should see a free service 'testservice-free-10to15yrs'", () => {
    cy.get('[data-testid="testservice-free-10to15yrs"]').should("exist");
});

//Paid and free
When("user filters using Paid and Free options", () => {
    cy.visit(`ProfessionalReferral/Search`);
    cy.searchbypostcode('BS2 0SP');
    cy.get('[data-testid="checkbox-paid"]').click();
    cy.get('[data-testid="checkbox-free"]').click();
    cy.get('[data-testid="button-apply-filters"]').click();
});

Then("the user should see a free service 'testservice-free-10to15yrs' and a paid service 'testservice-free-10to15yrs'", () => {
    cy.get('[data-testid="testservice-free-10to15yrs"]').should("exist");
    cy.get('[data-testid="testservice-paid-0to13yrs"]').should("exist");
});