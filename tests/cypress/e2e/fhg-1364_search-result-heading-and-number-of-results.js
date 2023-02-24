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

Then("the user should see {string} title", (title) => {
    cy.get('[data-testid="search-results-title"]').should("contain.text", title);
});

Then("the user should see {string} message", (results) => {

    cy.get('.search-results').find('[role="listitem"]').its('length').then((count)=>{
        var expectedString = results.replace('#', count);
        cy.get('[data-testid="number-of-results"]').should("contain.text", expectedString);
    });

});

Then("the user should see {string} poscode", (postcode) => {
    cy.get('[data-testid="postcodelink"]').should("contain.text", postcode);
});
