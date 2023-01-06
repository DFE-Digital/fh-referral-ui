import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user as arrived on search page", () => {
    cy.visit(`ProfessionalReferral/Search`);
}); 

When("user search with empty postcode", () => {
    cy.get('[data-testid="button-search"]').click();
});

Then("the user should see {string} empty error", (emptypostcode) => {
    cy.get('[data-testid="errorslist"]').should("exist"); 
    cy.get('[data-testid="empty-error"]').should("exist");
    cy.get('[data-testid="errorslist"]').should("contain.text", emptypostcode);
    cy.get('[data-testid="empty-error"]').should("contain.text", emptypostcode);
});

When("user search with invalid postcode {string}", (postcode) => {
    cy.get('[data-testid="postcode-value"]').type(postcode);
    cy.get('[data-testid="button-search"]').click();
});

Then("the user should see {string} invalid error", (invalidpostcode) => {
    cy.get('[data-testid="errorslist"]').should("exist");
    cy.get('[data-testid="invalid-error"]').should("exist");
    cy.get('[data-testid="errorslist"]').should("contain.text", invalidpostcode);
    cy.get('[data-testid="invalid-error"]').should("contain.text", invalidpostcode);
});

When("user search with a valid postcode {string}", (postcode) => {
    cy.get('[data-testid="postcode-value"]').type(postcode);
    cy.get('[data-testid="button-search"]').click();
});

Then("the user should see search results", () => {
    cy.get('[data-testid="testservice1"]').should("exist");
});


Then("the user should see no results", () => {
    cy.get('[data-testid="testservice1"]').should('not.exist');
});

//*** Back button
When("user clicks the back button", () => {
    cy.get('[data-testid="back-button"]').click();
});

Then("the application should navigate to Search page", () => {
    cy.location('pathname').should('match', new RegExp("/ProfessionalReferral/Search"));
});

//*** postcode hyperlink
When("user clicks the postcode hyperlink", () => {
    cy.visit(`ProfessionalReferral/Search`);
    cy.searchbypostcode('BS2 0SP');
    cy.get('[data-testid="postcodelink"]').click();
});

Then("the application should navigate to Search", () => {
    cy.location('pathname').should('match', new RegExp("/ProfessionalReferral/Search"));
});