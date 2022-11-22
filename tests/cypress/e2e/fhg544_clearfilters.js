import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on search page", () => {
    cy.visit(`ProfessionalReferral/Search`);
});
When("user searches using postcode {string}", (postcode) => {
    cy.searchbypostcode(postcode);
});
Then("user can see searvice {string}", (service) => {
    cy.get('[data-testid="'+ service +'"]').should("exist");
})
When("user filters using language {string}", (language) => {
    cy.get('[data-testid="select-language"]').select(language);
});
Then("the user can not see the service {string}", (service) => {
    cy.get('[data-testid="' + service + '"]').should("exist");
});
When("user hits clear filters", () => {
    cy.get('[data-testid="clearfilters"]').click();
});
Then("user can again see the service {string}", (service) => {
    cy.get('[data-testid="' + service + '"]').should("exist");
});