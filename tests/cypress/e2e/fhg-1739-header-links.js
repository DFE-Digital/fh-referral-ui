import { Given, Then } from "@badeball/cypress-cucumber-preprocessor";


Given("Given a user has started the referral ui journey", () => {
    cy.visit(``);
});

Then("Then the user should see and able to click on header phase banner feedback link form redirect to formthe user should see and able to click on feedback form redirect to form", () => {
    cy.get('[data-testid="phase-banner-feedback-link"]').should("exist");
    cy.get('[data-testid="phase-banner-feedback-link"]').should("have.attr", "href","https://dferesearch.fra1.qualtrics.com/jfe/form/SV_3911VGJ6TMfm8h8");

});