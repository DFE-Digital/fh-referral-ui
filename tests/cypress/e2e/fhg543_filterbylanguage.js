import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on search page", () => {
    cy.visit(`ProfessionalReferral/Search`);
});
When("user searches by a valid postcode {string}", (postcode) => {
    cy.searchbypostcode(postcode);
});
Then("the user should see language filter", () => {
    cy.get('[data-testid="select-language"]').should("exist");
})


When("user searches using postcode {string} and filters using language {string}", (postcode, language) => {
    cy.visit(`ProfessionalReferral/Search`);
    cy.searchbypostcode(postcode);
    cy.get('[data-testid="select-language"]').select(language);
    cy.get('[data-testid="button-apply-filters"]').click();
})
Then("the user should see the service {string}", (service) => {
    cy.get('[data-testid="' + service +'"]').should("exist");
});


When("user searches using postcode {string}", (postcode) => {
    cy.visit(`ProfessionalReferral/Search`);
    cy.searchbypostcode(postcode);
})
Then("the user should see the language {string} in language dropdown", (language) => {
    cy.get('[data-testid="select-language"]').select(language).should('have.value', language);
});