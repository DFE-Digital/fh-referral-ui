import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user as arrived on search page", () => {
    cy.visit(`ProfessionalReferral/Search`);
});

When("user search with a valid postcode {string}", (postcode) => {
    cy.get('[data-testid="radio-postcode"]').click();
    cy.get('[data-testid="postcode-value"]').type(postcode);
    cy.get('[data-testid="button-search"]').click();
});


Then("the user should see no results", () => {
    cy.get('[data-testid="aidforchildrenwithtracheostomies"]').should('not.exist');
});

//*** try different postcode hyperlink
When("user clicks the try using different postcode hyperlink", () => {
    cy.visit(`ProfessionalReferral/Search`);
    cy.searchbypostcode('WF8 1LD');
    cy.get('[data-testid="searchusingdifferentpostcodelink"]').click();
});

Then("the application should navigate to Search", () => {
    cy.location('pathname').should('match', new RegExp("/ProfessionalReferral/Search"));
});