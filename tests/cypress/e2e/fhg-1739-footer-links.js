import { Given, Then } from "@badeball/cypress-cucumber-preprocessor";


Given("a user has started the referral ui journey", () => {
    cy.visit(``);
});

Then("the user should see and able to click on  footer link {string} links to {string}", (id, url) => {
    cy.get(`[data-testid=${id}]`).should("exist");
    cy.get(`[data-testid=${id}]`).click();
    cy.location('pathname').should('match', new RegExp(url));
});

Then("the user should see and able to click on feedback form redirect to form", () => {
    cy.get('[data-testid="feedback-link"]').should("exist");
    cy.get('[data-testid="feedback-link"]').should("have.attr", "href","https://qfreeaccountssjc1.az1.qualtrics.com/jfe/form/SV_3aXYQWE1MKuhSya");

});