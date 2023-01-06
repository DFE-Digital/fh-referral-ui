import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";


Given("a user has arrived on search results page by a search with a valid postcode {string}", (postcode) => {
    cy.visit(`ProfessionalReferral/Search`);
    cy.get('[data-testid="postcode-value"]').type(postcode);
    cy.get('[data-testid="button-search"]').click();
});

When("user click on the  results", () => {
    cy.get('[data-testid="testservice1"]').click();
});

Then("search results page should show heading {string}", (heading) => {
    cy.get("h1").should("contain.text", heading);
});

Then("search results page should show cost has {string}", (cost) => {
    cy.get('[data-testid="cost-value"]').contains(cost)
});


Then("user clicks on the back button he should be redirected to search results page", () => {
    cy.get('[data-testid="back-button"]').click()
    cy.get('[data-testid="testservice1"]').should("exist");
});