import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";



Given("a user has arrived on search page", () => {
    cy.visit(`ProfessionalReferral/Search`);
});
When("user searches by a valid postcode {string}", (postcode) => {
    cy.searchbypostcode(postcode);
});

When("user searches using postcode {string} and filters using language {string}", (postcode, language) => {
    cy.visit(`ProfessionalReferral/Search`);
    cy.searchbypostcode(postcode);
    cy.get('[data-testid="select-language"]').select(language);
    cy.get('[data-testid="button-apply-filters"]').click();
})

Then("the user should see the language filter {language}", (language) => {
    cy.get(`[data-testid=${language}-remove]`).should("exist");
});
