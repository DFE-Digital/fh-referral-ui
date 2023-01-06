import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

var currentCategory = "";

Given("a user has arrived on search page", () => {
    cy.visit(`ProfessionalReferral/Search`);
});

When("user search with a valid postcode {string}", (postcode) => {
    cy.get('[data-testid="postcode-value"]').type(postcode);
    cy.get('[data-testid="button-search"]').click();
});

When("a user selects {string} category", (category) => {
    cy.get('[data-testid="' + category.toLowerCase().replaceAll(' ', '') +'"]').click();
    cy.get('[data-testid="button-apply-filters"]').click();
});

Then("{string} should be present in the cloud and results", (service) => {
    cy.get('[data-testid="' + service.toLowerCase().replaceAll(' ', '') + '-remove"]').should("exist");
    cy.get('[data-testid="' + service.toLowerCase().replaceAll(' ', '') + '-table"]').should("exist");
});
