import { Given, Then } from "@badeball/cypress-cucumber-preprocessor";


Given("a user has arrived on accessibility statement", () => {
    cy.visit(`/ProfessionalReferral/Accessibility`);
});

Then("the user should see the accessibility heading", () => {
    cy.get('[data-testid="accessibility-heading"]').should(
        "contain.text",
        "Accessibility Statement"
    );
});

Then("the user clicks on mailto link should popup with connect-family-support.service@education.gov.uk", () => {
    cy.get('[data-testid="mailto-connect-family-link"]').should("have.attr", "href", "mailto:connect-family-support.service@education.gov.uk");
});

Then("the user clicks on home button should redirect to home page", () => {
    cy.location('pathname').should('match', new RegExp("/"));
});

