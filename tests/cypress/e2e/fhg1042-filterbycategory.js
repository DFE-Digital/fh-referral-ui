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

When("a user selects Activities category", () => {
    cy.get('[data-testid="activities"]').click();
    cy.get('[data-testid="button-apply-filters"]').click();
});

Then("Aid for Children with Tracheostomies should not be in results", () => {
    cy.get('[data-testid="aidforchildrenwithtracheostomies"]').should("not.exist");
});

Then("Test Service - Paid - 15 to 20yrs - Afrikaans should be in results", () => {
    cy.get('[data-testid="testservice-paid-15to20yrs-afrikaans"]').should("exist");
}); 