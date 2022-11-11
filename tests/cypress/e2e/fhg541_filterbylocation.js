import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on search page", () => {
    cy.visit(`ProfessionalReferral/Search`);
});
When("user searches by a valid postcode {string}", (postcode) => {
    cy.searchbypostcode(postcode);
});
Then("the user should see family can choose location filter", () => {
    cy.get('[data-testid="checkbox-location"]').should("exist");
})



Given("a user has arrived on search results page using a valid postcode {string}", (postcode) => {
    cy.visit(`ProfessionalReferral/Search`);
    cy.searchbypostcode(postcode);
});
When("user filters using option for families to choose the location as checked", () => {
    cy.get('[data-testid="checkbox-location"]').click();
    cy.get('[data-testid="button-apply-filters"]').click();
})
Then("user is presented with local services that offer option for families to choose the location", () => {
    cy.get('[data-testid="testservice-paid-15to20yrs-afrikaans"]').should("exist");
})


