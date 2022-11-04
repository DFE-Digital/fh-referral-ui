import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on search page", () => {
    cy.visit(`ProfessionalReferral/Search`);
});
When("user searches by a valid postcode {string}", (postcode) => {
    cy.searchbypostcode(postcode);
});
Then("the user should see children and young people filter", () => {
    cy.get('[data-testid="checkbox-children"]').should("exist");
})



Given("a user has arrived on search results page using a valid postcode {string}", (postcode) => {
    cy.visit(`ProfessionalReferral/Search`);
    cy.searchbypostcode(postcode);
});
When("user filters using age = 14", () => {
    cy.get('[data-testid="checkbox-children"]').click();
    cy.get('[data-testid="select-children"]').select("14");
    cy.get('[data-testid="button-apply-filters"]').click();
})
Then("user is presented with local services that offer children and young people services for the selected age", () => {
    cy.get('[data-testid="testservice-free-10to15yrs"]').should("exist");
})


