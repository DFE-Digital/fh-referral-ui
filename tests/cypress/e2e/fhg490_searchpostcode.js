import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on search page", () => {
    cy.visit(`ProfessionalReferral/Search`);
});


When("user search by a postcode {string}", (postcode) => {
    cy.get('[data-testid="postcode-value"]').type(postcode);
    cy.get('[data-testid="button-search"]').click();
});

When("user search by a empty postcode", () => {
    cy.get('[data-testid="button-search"]').click();
});

Then("the user should see search results", () => {
    cy.get('[data-testid="aidforchildrenwithtracheostomies"]').should("exist");
});

Then("the user should see error message for empty postcode", () => {
    cy.get('[data-testid="empty-error"]').should("exist");
});

Then("the user should see error message for invalid postcode", () => {
    cy.get('[data-testid="invalid-error"]').should("exist");
});

When("user clicks the back button", () => {
    cy.get('[data-testid="back-button"]').click();
});

Then("the application should navigate to start page", () => {
    cy.location('pathname').should('match', new RegExp("/"));
});
