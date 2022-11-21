import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on search page", () => {
    cy.visit(`ProfessionalReferral/Search`);
});
When("user searches using postcode {string} and filters using language {string}", (postcode, language) => {
    cy.visit(`ProfessionalReferral/Search`);
    cy.searchbypostcode(postcode);
    cy.get('[data-testid="select-language"]').select(language);
    cy.get('[data-testid="button-apply-filters"]').click();
})
Then("the user should see the heading {string} and message {string}", (heading, message) => {
    cy.get('[data-testid="' + heading + '"]').should("exist");
    cy.get('[data-testid="' + message + '"]').should("exist");
});

